using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnifiedModel.SourceGenerator.CommonModels;

namespace UnifiedModel.SourceGenerator.OnChainModels.Ethereum
{
    public class Function : ChainModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("modifier")]
        public Modifiers Modifier { get; set; }

        [JsonProperty("parameters")]
        public string Parameters { get; set; }

        [JsonIgnore]
        public string ParameterAnchor { get; set; }

        [JsonProperty("expressions")]
        public IEnumerable<Expression> Expressions { get; set; }

        public Function(string name, Modifiers modifier, string parameters, string parameterAnchor, string parentHash)
        {
            Name = name;
            Modifier = modifier;
            Parameters = parameters;
            ParameterAnchor = parameterAnchor;
            ParentHash = parentHash;
            Expressions = new List<Expression>();
        }

        public override string ToString()
        {
            return $"function {Name}() {Modifier} {{\n" +
                $"{string.Join("\n", Expressions.Select(expression => expression.ToString()))}" +
                $"}}";
        }
    }
}