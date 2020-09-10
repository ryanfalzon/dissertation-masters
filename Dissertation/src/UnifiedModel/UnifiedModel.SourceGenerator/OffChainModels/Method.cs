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

        public IEnumerable<Expression> Expressions { get; set; }

        public Method(Modifiers modifier, string returnType, string identifier, string parentHash)
        {
            Modifier = modifier;
            ReturnType = returnType;
            Identifier = identifier;
            ParentHash = parentHash;
            Expressions = new List<Expression>();
        }

        public override string ToString()
        {
            return $"{Modifier} {ReturnType} {Identifier} ()\n" +
                $"{{\n" +
                $"{string.Join("\n", Expressions.Select(expression => expression.ToString()))}\n" +
                $"}}";
        }
    }
}