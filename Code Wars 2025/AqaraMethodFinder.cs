using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Code_Wars_2025
{
    public static class AqaraMethodFinder
    {
        public static async Task FindAqaraServiceCalls(string code, string interfaceLookin)
        {
            var tree = CSharpSyntaxTree.ParseText(code);
            var root = tree.GetRoot();

            var fields = root.DescendantNodes()
                .OfType<FieldDeclarationSyntax>()
                .Where(f =>
                    f.Modifiers.Any(m => m.IsKind(SyntaxKind.ReadOnlyKeyword)) &&
                    f.Declaration.Type is IdentifierNameSyntax type &&
                    type.Identifier.Text == interfaceLookin);

            foreach (var field in fields)
            {
                var fieldNames = field.Declaration.Variables.Select(v => v.Identifier.Text).ToList();
                var nameList = string.Join(", ", fieldNames);
                Console.WriteLine($"Znaleziono readonly pole IAqaraDevicesService: {nameList}");

                // Szukamy wywołań metod na tym polu
                var invocations = root.DescendantNodes()
                    .OfType<InvocationExpressionSyntax>()
                    .Where(invocation =>
                    {
                        if (invocation.Expression is MemberAccessExpressionSyntax memberAccess &&
                            memberAccess.Expression is IdentifierNameSyntax identifier)
                        {
                            return fieldNames.Contains(identifier.Identifier.Text);
                        }
                        return false;
                    });

                foreach (var invocation in invocations)
                {
                    var memberAccess = invocation.Expression as MemberAccessExpressionSyntax;
                    var methodName = memberAccess?.Name.Identifier.Text;
                    if (methodName != null)
                        Console.WriteLine($"  → Wywołanie metody: {methodName}()");
                }

                Console.WriteLine(); // pusty wiersz dla czytelności
            }
        }
    }
}