using Newtonsoft.Json;
using UnifiedModel.SourceGenerator.CommonModels;

namespace UnifiedModel.SourceGenerator.OffChainModels
{
    public class Expression : ChainModel
    {
        [JsonProperty("statement")]
        public string Statement { get; set; }

        public Expression(string statement, string parentHash)
        {
            Statement = statement;
            ParentHash = parentHash;

        }

        public override string ToString()
        {
            return $"{Statement}";
        }
    }
}