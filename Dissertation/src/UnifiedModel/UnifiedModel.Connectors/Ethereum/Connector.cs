using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;

namespace UnifiedModel.Connectors.Ethereum
{
    public class Connector
    {
        private readonly Web3 web3;
        private readonly string publicKey;
        private readonly string abi;
        private readonly string contractAddress;

        public Connector(string publicKey, string privateKey)
        {
            this.publicKey = publicKey;
            web3 = new Web3(new Account(privateKey));
        }

        public TransactionReceipt Call(string functionName, params object[] functionInput)
        {
            var contract = web3.Eth.GetContract(abi, contractAddress);
            var function = contract.GetFunction(functionName);

            var gas = function.EstimateGasAsync(publicKey, null, null, functionInput).Result;
            var result = function.SendTransactionAndWaitForReceiptAsync(publicKey, gas, null, null, functionInput: functionInput).Result;

            return result;
        }
    }
}