using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnifiedModel.SourceGenerator.CommonModels;
using UnifiedModel.SourceGenerator.Helpers;

namespace UnifiedModel.SourceGenerator.OnChainModels.Ethereum
{
    public class Constructor : ChainModel
    {
        [JsonProperty("modifier")]
        public Modifiers Modifier { get; set; }

        [JsonProperty("parameters")]
        public string Parameters { get; set; }

        [JsonIgnore]
        public string ParameterAnchor { get; set; }

        [JsonProperty("expressions")]
        public IEnumerable<Expression> Expressions { get; set; }

        public Constructor(Modifiers modifier, string parameters, string parameterAnchor, string parentHash)
        {
            Modifier = modifier;
            Parameters = parameters;
            ParameterAnchor = parameterAnchor;
            ParentHash = parentHash;
            Expressions = new List<Expression>();
        }

        public override string ToString()
        {
            Tools.IndentationLevel++;

            var content = $"constructor({Parameters}) {Modifier} {{\n".Tabulate() +
                $"{string.Join("\n", Expressions.Select(expression => expression.ToString()))}\n" +
                $"}}".Tabulate();

            Tools.IndentationLevel--;

            return content;
        }
    }
}