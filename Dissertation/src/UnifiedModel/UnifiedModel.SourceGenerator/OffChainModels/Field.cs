using UnifiedModel.SourceGenerator.CommonModels;

namespace UnifiedModel.SourceGenerator.OffChainModels
{
    public class Field: ChainModel
    {
        public Modifiers Modifier { get; set; }

        public Types Type { get; set; }

        public string Name { get; set; }

        public Field(Modifiers modifier, Types type, string name, string parentHash)
        {
            Modifier = modifier;
            Type = type;
            Name = name;
            ParentHash = parentHash;
        }

        public override string ToString()
        {
            return $"{Modifier} {Type} {Name} {{ get; set; }}\n";
        }
    }
}