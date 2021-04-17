using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using System;
using System.Linq;
using UnifiedModel.Connectors.Models;

namespace UnifiedModel.Connectors.Ethereum
{
    public class Connector
    {
        private readonly Web3 web3;
        private readonly EthereumSettings settings;

        public Connector(EthereumSettings settings)
        {
            this.settings = settings;
            web3 = new Web3(new Account(settings.PrivateKey));
        }

        public TransactionReceipt Call(string contractName, string functionName, params object[] functionInput)
        {
            var contractSettings = settings.Contracts.Where(contract => contract.Name.Equals(contractName)).FirstOrDefault();
            if(contractSettings == null)
            {
                throw new Exception("Contract not yet deployed!");
            }

            var contract = web3.Eth.GetContract(contractSettings.AbiLocation, contractSettings.Address);
            var function = contract.GetFunction(functionName);

            var gas = function.EstimateGasAsync(settings.PublicKey, null, null, functionInput).Result;
            var result = function.SendTransactionAndWaitForReceiptAsync(settings.PublicKey, gas, null, null, functionInput: functionInput).Result;

            return result;
        }
    }
}