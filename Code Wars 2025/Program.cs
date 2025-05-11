using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Reflection;

internal class Program
{
    private static bool ContainsRequest(string path)
    {
        var content = File.ReadAllText(path);

        return content.Contains("GetAsync") || content.Contains("PostAsync");
    }

    private static List<string> LookinToMethodsStart(string filePath)
    {
        var sourceCode = File.ReadAllText(filePath);

        // 1. Parsujemy kod źródłowy do SyntaxTree
        var tree = CSharpSyntaxTree.ParseText(sourceCode);
        var root = tree.GetRoot();

        // 2. Wyszukujemy wywołania GetAsync/PostAsync
        var invocations = root
            .DescendantNodes()
            .OfType<InvocationExpressionSyntax>()
            .Where(inv =>
            {
                if (inv.Expression is MemberAccessExpressionSyntax member)
                {
                    var name = member.Name.Identifier.Text;
                    return name == "GetAsync" || name == "PostAsync";
                }
                return false;
            });

        List<string> results = [];

        // 3. Dla każdego wywołania znajdujemy deklarację metody
        foreach (var inv in invocations)
        {
            var methodDecl = inv
                .Ancestors()
                .OfType<MethodDeclarationSyntax>()
                .FirstOrDefault();
            if (methodDecl != null)
            {
                var methodName = methodDecl.Identifier.Text;
                results.Add(methodName);
                Console.WriteLine($"Metoda: {methodName}, linia: {inv.GetLocation().GetLineSpan().StartLinePosition.Line + 1}");
            }
        }

        return results;
    }

    private static void Main(string[] args)
    {
        //var files = Directory.GetFiles("C:\\Users\\Jarek\\Desktop\\Istotne\\source\\Visual Studio\\Main\\SmartHome_BE", "*.cs", SearchOption.AllDirectories);

        var filesgg = Directory.GetFiles(@"C:\Users\Jarek\Desktop\Istotne\source\Visual Studio\Main\SmartHome_BE", "*.cs", SearchOption.AllDirectories).Where(ContainsRequest).ToList();

        foreach (var item in filesgg)
        {
            var methodsMy = LookinToMethodsStart(item);
        }

        var myText = "$\"https://polska.com/open/v1/project/fss\"\"?isAllDay=true&isCompleted=true\"\"ss\"";

        string exText = "myExampleBranch";

        var isCont = exText.Contains("branch");

        List<string> files = [@"C:\Users\Jarek\Desktop\Istotne\source\Visual Studio\Test\Code Wars 2025\Code Wars 2025\FilesToAnalysis\ExampleFileToAnalyse.cs"];

        var syntaxTrees = files.Select(file => CSharpSyntaxTree.ParseText(File.ReadAllText(file))).ToList();

        var references = new List<MetadataReference>
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location),
            MetadataReference.CreateFromFile(Assembly.Load("System.Runtime").Location)
        };

        var compilation = CSharpCompilation.Create("Analysis", syntaxTrees, references);
        foreach (var tree in syntaxTrees)
        {
            var semanticModel = compilation.GetSemanticModel(tree);
            var root = tree.GetRoot();

            var variableAssignments = new Dictionary<string, string>();

            var variableDeclaratorSyntaxes = root.DescendantNodes().OfType<VariableDeclaratorSyntax>().ToList();

            foreach (var declarator in variableDeclaratorSyntaxes)
            {
                if (declarator.Initializer != null)
                {
                    var name = declarator.Identifier.Text;
                    var value = declarator.Initializer.Value.ToString();
                    if (!variableAssignments.ContainsKey(name))
                        variableAssignments[name] = value;
                }
            }

            var expressionStatementSyntaxes = root.DescendantNodes().OfType<ExpressionStatementSyntax>().ToList();

            foreach (var assignment in root.DescendantNodes().OfType<ExpressionStatementSyntax>())
            {
                if (assignment.Expression is AssignmentExpressionSyntax assignExpr && assignExpr.IsKind(SyntaxKind.AddAssignmentExpression))
                {
                    if (assignExpr.Left is IdentifierNameSyntax identifier && assignExpr.Right is ExpressionSyntax right)
                    {
                        var name = identifier.Identifier.Text;
                        var value = right.ToString();
                        if (variableAssignments.ContainsKey(name))
                        {
                            variableAssignments[name] += value;
                        }
                    }
                }
            }

            var invocations = root.DescendantNodes()
                .OfType<InvocationExpressionSyntax>()
                .Where(inv =>
                    inv.Expression is MemberAccessExpressionSyntax member &&
                    (member.Name.ToString().EndsWith("GetAsync") ||
                     member.Name.ToString().EndsWith("PostAsync") ||
                     member.Name.ToString().EndsWith("PutAsync") ||
                     member.Name.ToString().EndsWith("DeleteAsync")))
                .ToList();

            if (invocations.Count > 0)
            {
            }

            foreach (var invocation in invocations)
            {
                var member = (MemberAccessExpressionSyntax)invocation.Expression;
                var methodName = member.Name.ToString();
                var argument = invocation.ArgumentList.Arguments.FirstOrDefault();
                string argumentStr = argument?.ToString() ?? "<no argument>";

                if (argument?.Expression is IdentifierNameSyntax identifier)
                {
                    if (variableAssignments.TryGetValue(identifier.Identifier.Text, out var assignedValue))
                    {
                        int count = assignedValue.Count(c => c == '"');

                        var valueMy = assignedValue.Replace("\"", string.Empty);

                        argumentStr = valueMy;
                    }
                }
                else if (argument?.Expression is InvocationExpressionSyntax methodCall)
                {
                    var symbol = semanticModel.GetSymbolInfo(methodCall).Symbol as IMethodSymbol;
                    if (symbol != null && symbol.DeclaringSyntaxReferences.Length > 0)
                    {
                        var methodDecl = symbol.DeclaringSyntaxReferences[0].GetSyntax() as MethodDeclarationSyntax;
                        var returnStmt = methodDecl?.Body?.Statements.OfType<ReturnStatementSyntax>().FirstOrDefault();
                        if (returnStmt?.Expression != null)
                        {
                            argumentStr = returnStmt.Expression.ToString();
                        }
                    }
                }
                string text = $"[{methodName.Replace("Async", "").ToUpper()}] {argumentStr} in {Path.GetFileName(tree.FilePath)}";
                if (!string.IsNullOrEmpty(text))
                {
                }
                Console.WriteLine(text);
            }
        }
    }
}