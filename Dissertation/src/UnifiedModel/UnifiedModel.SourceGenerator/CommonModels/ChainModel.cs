using Newtonsoft.Json;

namespace UnifiedModel.SourceGenerator.CommonModels
{
    public abstract class ChainModel
    {
        [JsonIgnore]
        public string ParentHash { get; set; }

        [JsonIgnore]
        public string Hash { get; set; }
    }
}