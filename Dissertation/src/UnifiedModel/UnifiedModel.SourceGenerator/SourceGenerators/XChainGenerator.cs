using UnifiedModel.SourceGenerator.OffChainModels;

namespace UnifiedModel.SourceGenerator.SourceGenerators
{
    public interface XChainGenerator
    {
        void AddClass(Modifiers modifier, string name);

        void AddField(Modifiers modifier, Types type, string name);

        string Consume();
    }
}