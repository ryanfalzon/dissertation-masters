using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnifiedModel.SourceGenerator.CommonModels;
using UnifiedModel.SourceGenerator.Helpers;

namespace UnifiedModel.SourceGenerator.OffChainModels
{
    public class Class: ChainModel
    {
        [JsonProperty("modifier")]
        public Modifiers Modifier { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("isModel")]
        public bool IsModel { get; set; }

        [JsonProperty("fields")]
        public IEnumerable<Field> Fields { get; set; }

        [JsonProperty("method")]
        public IEnumerable<Method> Methods { get; set; }

        public Class(Modifiers modifier, string name, bool isModel, string parentHash)
        {
            Modifier = modifier;
            Name = name;
            ParentHash = parentHash;
            IsModel = isModel;
            Fields = new List<Field>();
            Methods = new List<Method>();
        }

        public override string ToString()
        {
            Tools.IndentationLevel = 0;

            var content =  $"{Modifier} class {Name}\n".Tabulate() +
                $"{{\n".Tabulate() +
                $"{string.Join("\n", Fields.Select(field => field.ToString()))}\n" +
                $"{string.Join("\n", Methods.Select(field => field.ToString()))}\n" +
                $"}}".Tabulate();

            Tools.IndentationLevel = 0;

            return content;
        }
    }
}