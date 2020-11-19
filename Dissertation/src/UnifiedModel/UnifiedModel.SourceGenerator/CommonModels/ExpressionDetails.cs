using Microsoft.CodeAnalysis.CSharp;

namespace UnifiedModel.SourceGenerator.CommonModels
{
    public class ExpressionDetails : NodeDetails
    {
        public SyntaxKind SyntaxKind { get; set; }

        public string Statement { get; set; }
    }
}