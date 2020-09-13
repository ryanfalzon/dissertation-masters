using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnifiedModel.SourceGenerator.CommonModels;

namespace UnifiedModel.SourceGenerator.OffChainModels
{
    public class Method : ChainModel
    {
        [JsonProperty("modifier")]
        public Modifiers Modifier { get; set; }

        [JsonProperty("returnType")]
        public string ReturnType { get; set; }

        [JsonProperty("identifier")]
        public string Identifier { get; set; }

        [JsonProperty("parameters")]
        public string Parameters { get; set; }

        [JsonProperty("expressions")]
        public IEnumerable<Expression> Expressions { get; set; }

        public Method(Modifiers modifier, string returnType, string identifier, string parameters, string parentHash)
        {
            Modifier = modifier;
            ReturnType = returnType;
            Identifier = identifier;
            Parameters = parameters;
            ParentHash = parentHash;
            Expressions = new List<Expression>();
        }

        public override string ToString()
        {
            return $"{Modifier} {ReturnType} {Identifier} ({Parameters})\n" +
                $"{{\n" +
                $"{string.Join("\n", Expressions.Select(expression => expression.ToString()))}\n" +
                $"}}";
        }
    }
}