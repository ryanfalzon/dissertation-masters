using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnifiedModel.SourceGenerator.CommonModels;

namespace UnifiedModel.SourceGenerator.OnChainModels.Ethereum
{
    public class Struct : ChainModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("properties")]
        public IEnumerable<Property> Properties { get; set; }

        public Struct(string name, string parentHash)
        {
            Name = name;
            ParentHash = parentHash;
            Properties = new List<Property>();
        }

        public override string ToString()
        {
            return $"struct {Name}{{\n" +
                $"{string.Join("\n", Properties.Select(property => property.ToString()))}\n" +
                $"}}";
        }
    }
}