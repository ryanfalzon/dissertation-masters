using UnifiedModel.SourceGenerator.OffChainModels;

namespace UnifiedModel.SourceGenerator.SourceGenerators
{
    public interface XChainGenerator
    {
        string AddClass(Modifiers modifier, string name, string parentHash);

        string AddField(Modifiers modifier, Types type, string name, string parentHash);

        void Consume();

        string ToString();
    }
}