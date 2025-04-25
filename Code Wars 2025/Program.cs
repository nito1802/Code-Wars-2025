using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Code_Wars_2025
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var files = Directory.GetFiles("C:\\Users\\Jarek\\Desktop\\Istotne\\source\\Visual Studio\\Main\\SmartHome_BE", "*.cs", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                var code = File.ReadAllText(file);
                var tree = CSharpSyntaxTree.ParseText(code);
                var root = tree.GetRoot();

                var variableAssignments = new Dictionary<string, string>();

                // Get initial assignments
                foreach (var declarator in root.DescendantNodes().OfType<VariableDeclaratorSyntax>())
                {
                    if (declarator.Initializer != null)
                    {
                        var name = declarator.Identifier.Text;
                        var value = declarator.Initializer.Value.ToString();
                        if (!variableAssignments.ContainsKey(name))
                            variableAssignments[name] = value;
                    }
                }

                // Handle += style augmentations
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
                         member.Name.ToString().EndsWith("DeleteAsync")));

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
                            argumentStr = assignedValue;
                        }
                    }

                    Console.WriteLine($"[{methodName.Replace("Async", "").ToUpper()}] {argumentStr} in {Path.GetFileName(file)}");
                }
            }

            var span = new DateTime(2025, 04, 19, 8, 00, 0) - new DateTime(2025, 04, 19, 6, 30, 0);

            var text = span.ToString(@"hh\:mm\:ss");
        }
    }
}