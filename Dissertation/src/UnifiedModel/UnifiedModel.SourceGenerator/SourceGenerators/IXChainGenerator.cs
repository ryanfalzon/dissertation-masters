using UnifiedModel.SourceGenerator.CommonModels;

namespace UnifiedModel.SourceGenerator.SourceGenerators
{
    public interface IXChainGenerator
    {
        string AddClass(Modifiers modifier, string name, bool isModel, string parentHash);

        string AddField(Modifiers modifier, Types type, string name, string parentHash);

        string AddMethod(Modifiers modifier, string returnType, string identifier, string parentHash);

        string AddExpression(string statement, string parentHash);

        void Consume();
    }
}