using Neo;
using Neo.Network.P2P.Payloads;
using Neo.Network.RPC;
using Neo.Network.RPC.Models;
using Neo.SmartContract;
using Neo.Wallets;
using System.Security.Cryptography;

namespace Support
{
    public static class Helper
    {
        public static void SignAndSendTx(RpcClient _rpcClient, byte[] script, Signer[] signers, TransactionAttribute[] transactionAttributes = null, params KeyPair[] keyPair)
        {
            TransactionManagerFactory factory = new TransactionManagerFactory(_rpcClient);
            TransactionManager manager = factory.MakeTransactionAsync(script, signers, transactionAttributes).Result;

            foreach (var kp in keyPair)
            {
                manager.AddSignature(kp);
            }

            Transaction invokeTx = manager.SignAsync().Result;

            _rpcClient.SendRawTransactionAsync(invokeTx);

            Console.WriteLine($"Transaction {invokeTx.Hash} is broadcasted!");
        }

        public static string InvokeScript(RpcClient _rpcClient, byte[] script, params Signer[] signers)
        {
            RpcInvokeResult invokeResult = _rpcClient.InvokeScriptAsync(script, signers).Result;

            Console.WriteLine($"Invoke result: {invokeResult.ToJson()}");
            return invokeResult.ToJson()["stack"][0]["value"].GetString();
        }

        public static UInt160 GetScriptHash(this KeyPair keyPair) => Contract.CreateSignatureContract(keyPair.PublicKey).ScriptHash;



        public static UInt160 GetRandomAccount()
        {
            byte[] privateKey = new byte[32];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(privateKey);
            }
            KeyPair key = new KeyPair(privateKey);

            var contract = Contract.CreateSignatureContract(key.PublicKey);
            //var publicKey = key.PublicKey.ToString();
            //var wif = key.Export();
            //var address = contract.ScriptHash.ToAddress(ProtocolSettings.Default.AddressVersion);
            Console.WriteLine(contract.ScriptHash);
            return contract.ScriptHash;
        }
    }
}

