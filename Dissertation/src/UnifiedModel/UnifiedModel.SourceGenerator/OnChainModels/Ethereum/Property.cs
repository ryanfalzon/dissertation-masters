using Newtonsoft.Json;
using UnifiedModel.SourceGenerator.CommonModels;

namespace UnifiedModel.SourceGenerator.OnChainModels.Ethereum
{
    public class Property : ChainModel
    {
        [JsonProperty("modifier")]
        public Modifiers Modifier { get; set; }

        [JsonProperty("type")]
        public Types Type { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        public Property(Types type, string name, string parentHash)
        {
            Modifier = Modifiers.@public;
            Type = type;
            Name = name;
            ParentHash = parentHash;
        }

        public Property(Modifiers modifier, Types type, string name, string parentHash)
        {
            Modifier = modifier;
            Type = type;
            Name = name;
            ParentHash = parentHash;
        }

        public override string ToString()
        {
            return $"{Modifier} {Type} {Name};";
        }
    }
}