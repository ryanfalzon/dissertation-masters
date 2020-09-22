using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnifiedModel.SourceGenerator.CommonModels;
using UnifiedModel.SourceGenerator.Helpers;

namespace UnifiedModel.SourceGenerator.OnChainModels.Ethereum
{
    public class Struct : ChainModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("properties")]
        public IEnumerable<Property> Properties { get; set; }

        [JsonIgnore]
        public ModelProperties ModelProperties { get; set; }

        public Struct(string name, ModelProperties modelProperties, string parentHash)
        {
            Name = name;
            ParentHash = parentHash;
            ModelProperties = modelProperties;
            Properties = new List<Property>();
        }

        public override string ToString()
        {
            Tools.IndentationLevel++;

            var content = $"struct {Name}{{\n".Tabulate() +
                $"{string.Join("\n", Properties.Select(property => property.ToString()))}\n" +
                $"}}".Tabulate();

            Tools.IndentationLevel--;

            return content;
        }
    }
}