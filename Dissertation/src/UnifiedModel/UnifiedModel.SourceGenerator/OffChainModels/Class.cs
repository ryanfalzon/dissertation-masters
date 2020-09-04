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
        public List<Field> Fields { get; set; }

        public Class()
        {
            Fields = new List<Field>();
        }

        public Class(Modifiers modifier, string name, string parentHash)
        {
            Modifier = modifier;
            Name = name;
            ParentHash = parentHash;
            Fields = new List<Field>();
        }

        public override string ToString()
        {
            return $"{Modifier} class {Name} {{ \n{string.Join("\n", Fields.Select(field => field.ToString()))} }}";
        }
    }
}