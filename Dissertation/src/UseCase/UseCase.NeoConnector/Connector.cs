using Neo;
using Neo.Network.P2P.Payloads;
using Neo.Network.RPC;
using Neo.SmartContract;
using Neo.VM;
using Neo.Wallets;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace UseCase.NeoConnector
{
    public static class Connector
    {
        public static string SmartContractHash = "0x5e86a0a3af0a2ba4f7a724cbb63ece75f89d514a";

        public static string CallContractFunction(string senderPrivateKey, string operation, params object[] param)
        {
            try
            {
                // Create a new RPC client that is connected to a private test network
                RpcClient client = new RpcClient("http://localhost:10332");

                // Initialize the script that will be executed by the neo VM
                UInt160 contractHash = UInt160.Parse(SmartContractHash);
                byte[] script = contractHash.MakeScript(operation, param);

                // Generate a keypair from the sender's private key
                KeyPair senderKeyPair = Neo.Network.RPC.Utility.GetKeyPair(senderPrivateKey);
                UInt160 sender = Contract.CreateSignatureContract(senderKeyPair.PublicKey).ScriptHash;
                Signer signer = new Signer()
                {
                    Scopes = WitnessScope.CalledByEntry,
                    Account = sender
                };

                var result = client.InvokeScript(script, signer);
                return result.Stack.Single().GetString();
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        public static async Task BroadcastTransaction(string senderPrivateKey, string operation, params object[] param)
        {
            try
            {
                // Create a new RPC client that is connected to a private test network
                RpcClient client = new RpcClient("http://localhost:10332");

                // Initialize the script that will be executed by the neo VM
                UInt160 contractHash = UInt160.Parse(SmartContractHash);
                byte[] script = contractHash.MakeScript(operation, param);

                // Generate a keypair from the sender's private key
                KeyPair senderKeyPair = Neo.Network.RPC.Utility.GetKeyPair(senderPrivateKey);
                UInt160 sender = Contract.CreateSignatureContract(senderKeyPair.PublicKey).ScriptHash;
                Signer[] signers = new[]
                {
                    new Signer()
                    {
                        Scopes = WitnessScope.CalledByEntry,
                        Account = sender
                    }
                };

                // Create and broadcast the transaction
                Transaction transaction = new TransactionManager(client)
                    .MakeTransaction(script, signers, null)
                    .AddSignature(senderKeyPair)
                    .Sign()
                    .Tx;
                client.SendRawTransaction(transaction);

                // Wait until transaction is confirmed on the chain
                WalletAPI wallet = new WalletAPI(client);
                await wallet.WaitTransaction(transaction).ContinueWith(async (p) =>
                {
                    Console.WriteLine($"Transaction vm state is  {(await p).VMState}");
                });
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static byte[] DerivePublicKey(string privateKey)
        {
            KeyPair senderKeyPair = Neo.Network.RPC.Utility.GetKeyPair(privateKey);
            return senderKeyPair.PublicKey.ToString().HexToBytes();
        }
    }
}