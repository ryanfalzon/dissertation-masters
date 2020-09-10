using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnifiedModel.SourceGenerator.CommonModels;

namespace UnifiedModel.SourceGenerator.OffChainModels
{
    public class Class: ChainModel
    {
        [JsonProperty("modifier")]
        public Modifiers Modifier { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("fields")]
        public IEnumerable<Field> Fields { get; set; }

        [JsonProperty("method")]
        public IEnumerable<Method> Methods { get; set; }

        public Class(Modifiers modifier, string name, string parentHash)
        {
            Modifier = modifier;
            Name = name;
            ParentHash = parentHash;
            Fields = new List<Field>();
            Methods = new List<Method>();
        }

        public override string ToString()
        {
            return $"{Modifier} class {Name}{{\n" +
                $"{string.Join("\n", Fields.Select(field => field.ToString()))}\n" +
                $"{string.Join("\n", Methods.Select(field => field.ToString()))}\n" +
                $"}}";
        }
    }
}