using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnifiedModel.SourceGenerator.CommonModels;
using UnifiedModel.SourceGenerator.Helpers;

namespace UnifiedModel.SourceGenerator.OffChainModels.Desktop
{
    public class Constructor : ChainModel
    {
        [JsonProperty("modifier")]
        public Modifiers Modifier { get; set; }

        [JsonProperty("Identifier")]
        public string Identifier { get; set; }

        [JsonProperty("parameters")]
        public string Parameters { get; set; }

        [JsonIgnore]
        public string ParameterAnchor { get; set; }

        [JsonProperty("expressions")]
        public IEnumerable<Expression> Expressions { get; set; }

        public Constructor(Modifiers modifier, string identifier, string parameters, string parameterAnchor, string parentHash)
        {
            Modifier = modifier;
            Identifier = identifier;
            Parameters = parameters;
            ParameterAnchor = parameterAnchor;
            ParentHash = parentHash;
            Expressions = new List<Expression>();
        }

        public override string ToString()
        {
            Tools.IndentationLevel++;

            var content = $"{Modifier} {Identifier} ({Parameters})\n".Tabulate() +
                $"{{\n".Tabulate() +
                $"{string.Join("\n", Expressions.Select(expression => expression.ToString()))}\n" +
                $"}}".Tabulate();

            Tools.IndentationLevel--;

            return content;
        }
    }
}