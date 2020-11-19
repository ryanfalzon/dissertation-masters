using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnifiedModel.SourceGenerator.CommonModels;
using UnifiedModel.SourceGenerator.Helpers;

namespace UnifiedModel.SourceGenerator.OffChainModels.Desktop
{
    public class Class: ChainModel
    {
        [JsonProperty("modifier")]
        public Modifiers Modifier { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonIgnore]
        public ModelProperties ModelProperties { get; set; }

        [JsonProperty("fields")]
        public IEnumerable<Field> Fields { get; set; }

        [JsonProperty("constructors")]
        public IEnumerable<Constructor> Constructors { get; set; }

        [JsonProperty("method")]
        public IEnumerable<Method> Methods { get; set; }

        public Class(Modifiers modifier, string name, ModelProperties modelProperties, string parentHash)
        {
            Modifier = modifier;
            Name = name;
            ParentHash = parentHash;
            ModelProperties = modelProperties;
            Fields = new List<Field>();
            Constructors = new List<Constructor>();
            Methods = new List<Method>();
        }

        public override string ToString()
        {
            Tools.IndentationLevel = 0;

            var content =  $"{(!ModelProperties.IsModel ? Constants.DesktopUsingStatements : string.Empty)}\n" +
                $"{Modifier} class {Name}\n".Tabulate() +
                $"{{\n".Tabulate() +
                $"{string.Join("\n", Fields.Select(field => field.ToString()))}\n" +
                $"{string.Join("\n", Constructors.Select(constructor => constructor.ToString()))}\n" +
                $"{string.Join("\n", Methods.Select(method => method.ToString()))}\n" +
                $"}}".Tabulate();

            Tools.IndentationLevel = 0;

            return content;
        }
    }
}