using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnifiedModel.SourceGenerator.CommonModels;
using UnifiedModel.SourceGenerator.Helpers;

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

        [JsonProperty("returnTypes")]
        public string ReturnTypes { get; set; }

        [JsonProperty("expressions")]
        public IEnumerable<Expression> Expressions { get; set; }

        public Function(string name, Modifiers modifier, string parameters, string parameterAnchor, string returnTypes, string parentHash)
        {
            Name = name;
            Modifier = modifier;
            Parameters = parameters;
            ParameterAnchor = parameterAnchor;
            ReturnTypes = returnTypes;
            ParentHash = parentHash;
            Expressions = new List<Expression>();
        }

        public override string ToString()
        {
            Tools.IndentationLevel++;

            var formatedReturnTypes = ReturnTypes.Equals("void") ? string.Empty : $" returns ({ReturnTypes}) ";
            var content = $"function {Name}({Parameters}) {Modifier}{formatedReturnTypes}{{\n".Tabulate() +
                $"{string.Join("\n", Expressions.Select(expression => expression.ToString()))}\n" +
                $"}}".Tabulate();

            Tools.IndentationLevel--;

            return content;
        }
    }
}