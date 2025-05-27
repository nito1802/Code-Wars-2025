using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Code_Wars_2025
{
    public static class AqaraMethodFinder
    {
        public static async Task FindAqaraServiceCalls(string code)
        {
            var tree = CSharpSyntaxTree.ParseText(code);
            var root = tree.GetRoot();

            var fields = root.DescendantNodes()
                .OfType<FieldDeclarationSyntax>()
                .Where(f =>
                    f.Modifiers.Any(m => m.IsKind(SyntaxKind.ReadOnlyKeyword)) &&
                    f.Declaration.Type is IdentifierNameSyntax type &&
                    type.Identifier.Text == "IAqaraDevicesService");

            foreach (var field in fields)
            {
                var name = string.Join(", ", field.Declaration.Variables.Select(v => v.Identifier.Text));
                Console.WriteLine($"Znaleziono readonly pole IAqaraDevicesService: {name}");
            }
        }
    }
}