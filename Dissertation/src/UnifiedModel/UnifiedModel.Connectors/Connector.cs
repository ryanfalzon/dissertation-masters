using Newtonsoft.Json;
using System;
using UnifiedModel.Connectors.Models;

namespace UnifiedModel.Connectors
{
    public static class Connector
    {
        public static dynamic XCall(string location, string contractName, string functionName, params object[] functionInput)
        {
            switch (location)
            {
                case Locations.Ethereum:
                    {
                        var ethereumConnector = new Ethereum.Connector(JsonConvert.DeserializeObject<EthereumSettings>(Properties.Resources.Ethereum.ToString()));
                        ethereumConnector.Call(contractName, functionName, functionInput);
                    } break;
                default: throw new Exception("Passed location is not yet supported by connector library!");
            }

            return null;
        }
    }
}