using Newtonsoft.Json;
using System.Collections.Generic;

namespace UnifiedModel.Connectors.Models
{
    public class EthereumSettings
    {
        [JsonProperty("PublicKey")]
        public string PublicKey { get; set; }

        [JsonProperty("PrivateKey")]
        public string PrivateKey { get; set; }

        [JsonProperty("Contracts")]
        public List<EthereumContract> Contracts { get; set; }

        public class EthereumContract
        {
            public string Name { get; set; }

            public string AbiLocation { get; set; }

            public string Address { get; set; }
        }
    }
}