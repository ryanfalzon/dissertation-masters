using UnifiedModel.SourceGenerator.CommonModels;

namespace UnifiedModel.SourceGenerator.SourceGenerators
{
    public interface IXChainGenerator
    {
        string FileExtension { get; set; }

        string AddClass(ClassDetails classDetails, string parentHash);

        string AddField(FieldDetails fieldDetails, string parentHash);

        string AddMethod(MethodDetails methodDetails, string parentHash);

        string AddExpression(ExpressionDetails expressionDetails, string parentHash);

        string CreatePropertyArgument(string hash);

        void Consume();
    }
}