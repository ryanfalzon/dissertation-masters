using Newtonsoft.Json;
using UnifiedModel.SourceGenerator.CommonModels;
using UnifiedModel.SourceGenerator.Helpers;

namespace UnifiedModel.SourceGenerator.OffChainModels.Desktop
{
    public class Field: ChainModel
    {
        [JsonProperty("modifier")]
        public Modifiers Modifier { get; set; }

        [JsonProperty("type")]
        public Types Type { get; set; }

        [JsonProperty("name")]
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
            Tools.IndentationLevel++;

            var content = $"{Modifier} {Type} {Name} {{ get; set; }}\n".Tabulate();

            Tools.IndentationLevel--;

            return content;
        }
    }
}