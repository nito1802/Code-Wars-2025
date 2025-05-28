using Code_Wars_2025;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text.RegularExpressions;

public class EndpointCall
{
    /// <summary>
    /// Treść endpointa, np. "https://api.example.com/open/v1/task"
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// Get, Post, Put, Delete
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// W jakiej metodzie jest wywołanie, np. "GetRpipeAsync"
    /// </summary>
    public string MethodName { get; set; }

    /// <summary>
    /// Jaki projekt, np Documents.be
    /// </summary>
    public string Project { get; set; }

    public string FileName { get; set; }
    public string FullPath { get; set; }
}

internal class Programc
{
    public static List<string> ExtractParameters(string url)
    {
        var matches = Regex.Matches(url, @"\{([^{}]+)\}");
        var result = new List<string>();
        foreach (Match match in matches)
        {
            result.Add(match.Groups[1].Value);
        }
        return result;
    }

    private static async Task Main(string[] args)
    {
        var myPath = @"C:\Users\Jarek\Desktop\Istotne\source\Visual Studio\Main\MySmartHomeApp\MySmartHomeApp";

        Directory.GetFiles(myPath, "*.cs", SearchOption.AllDirectories)
            .ToList()
            .ForEach(Console.WriteLine);

        int alfa = 1;
        string beta = "polsk";

        var myString = "https://api.ticktick.com/open/v1/project/{alfa}/task/{beta}/completexx";

        var elemets = ExtractParameters(myString);

        var aqaraServiceCalls = File.ReadAllText(@"C:\Users\Jarek\Desktop\Istotne\source\Visual Studio\Test\Code Wars 2025\Code Wars 2025\FilesToAnalysis\CallbacksController.cs");

        await AqaraMethodFinder.FindAqaraServiceCalls(aqaraServiceCalls, "IAqaraDevicesService");

        string path = @"C:\Users\Jarek\Desktop\Istotne\source\Visual Studio\Test\Code Wars 2025\Code Wars 2025\FilesToAnalysis\ExampleFileToAnalyse.cs";

        var methodsNames = LookinToMethodsStart(path);
        var syntaxNode = await GetSyntaxNodeMy(path);

        var endpoints = await GetEndpointInvokationsFromFile(methodsNames, syntaxNode, path);

        foreach (var item in endpoints)
        {
            Console.WriteLine(item.Url);
        }
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
            }
        }

        return results;
    }

    private static async Task<SyntaxNode> GetSyntaxNodeMy(string filePath)
    {
        var code = File.ReadAllText(filePath);
        var tree = CSharpSyntaxTree.ParseText(code);
        var root = await tree.GetRootAsync();
        return root;
    }

    private static async Task<List<EndpointCall>> GetEndpointInvokationsFromFile(List<string> methodsWithRequests, SyntaxNode root, string filePath)
    {
        List<EndpointCall> results = new List<EndpointCall>();

        // Znajdź metodę CompleteTaskAsync
        var methods = root.DescendantNodes()
            .OfType<MethodDeclarationSyntax>()
            .Where(m => methodsWithRequests.Contains(m.Identifier.Text))
            .ToList();

        if (methods == null)
        {
            Console.WriteLine("Metoda nie znaleziona.");
            return null;
        }

        foreach (var method in methods)
        {
            // Słownik zmiennych lokalnych i ich wartości
            var variableAssignments = new Dictionary<string, string>();

            // Szukaj tylko wewnątrz ciała metody
            var bodyNodes = method.Body?.DescendantNodes().ToList();
            if (bodyNodes == null)
            {
                Console.WriteLine("Brak ciała metody.");
                return null;
            }

            // Deklaracje zmiennych
            var variableDeclaratorSyntaxes = bodyNodes
                .OfType<VariableDeclaratorSyntax>();

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

            var expressionStatementSyntaxes = method.DescendantNodes().OfType<ExpressionStatementSyntax>().ToList();

            foreach (var assignment in method.DescendantNodes().OfType<ExpressionStatementSyntax>())
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

            var invocations = method.DescendantNodes()
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

                string text = $"[{methodName.Replace("Async", "").ToUpper()}] {argumentStr} in {filePath}";

                results.Add(new()
                {
                    Url = argumentStr,
                    Type = methodName.Replace("Async", "").ToUpper(),
                    MethodName = method.Identifier.Text,
                    Project = "Documents.be",
                    FileName = Path.GetFileNameWithoutExtension(filePath),
                    FullPath = filePath
                });
            }
        }
        return results;
    }
}