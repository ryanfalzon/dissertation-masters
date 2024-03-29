﻿using Newtonsoft.Json;
using UnifiedModel.SourceGenerator.CommonModels;
using UnifiedModel.SourceGenerator.Helpers;

namespace UnifiedModel.SourceGenerator.OnChainModels.Ethereum
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
            Tools.IndentationLevel++;

            var content = $"{Statement}".Tabulate();

            Tools.IndentationLevel--;

            return content;
        }
    }
}