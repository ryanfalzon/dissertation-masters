using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnifiedModel.SourceGenerator.CommonModels;
using UnifiedModel.SourceGenerator.Helpers;

namespace UnifiedModel.SourceGenerator.OnChainModels.Ethereum
{
    public class Contract : ChainModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("properties")]
        public IEnumerable<Property> Properties { get; set; }

        [JsonProperty("functions")]
        public IEnumerable<Function> Functions{ get; set; }

        public Contract(string name, string parentHash)
        {
            Name = name;
            ParentHash = parentHash;
            Properties = new List<Property>();
            Functions = new List<Function>();
        }

        public override string ToString()
        {
            Tools.IndentationLevel = 0;

            var content = $"pragma solidity >=0.4.22 <0.7.0;\n".Tabulate() +
                $"contract {Name}{{\n".Tabulate() +
                $"{string.Join("\n", Properties.Select(property => property.ToString()))}\n" +
                $"{string.Join("\n", Functions.Select(function => function.ToString()))}\n" +
                $"}}".Tabulate();

            Tools.IndentationLevel = 0;

            return content;
        }
    }
}