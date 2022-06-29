using Neo;
using Neo.IO;
using Neo.SmartContract;
using Neo.Wallets;
using System;
using System.Linq;
using System.Text;
using Neo.Network.RPC;
using System.Security.Cryptography;
using Neo.SmartContract.Manifest;
using System.IO;
using Neo.VM;
using Neo.Network.P2P.Payloads;
using System.Numerics;
using Newtonsoft.Json.Linq;
using Neo.SmartContract.Native;
using Neo.VM.Types;

namespace Support
{
    class Program
    {
        //private static RpcClient _rpcClient = new RpcClient(new Uri("https://n3seed2.ngd.network:10332"), null, null, ProtocolSettings.Load("config.json", true));
        private static RpcClient _rpcClient = new RpcClient(new Uri("http://seed1t5.neo.org:20332"), null, null, ProtocolSettings.Load("config.json", true));

        private static KeyPair keyPair = Neo.Network.RPC.Utility.GetKeyPair("L5kyCkYVsAJu488Df6xnmeNYcnHi5uHNrYUUVwo8X2MqcrjV3QqQ");

        private static UInt160 ilex = UInt160.Parse("0x3a8aff90be1cf25d715dc3c79e729cd205f749b4");

        static void Main()
        {
            //DeployContract("/Users/yinwei/work/neo/3/ilex/ilexNft/bin/sc/", "ilex");
            //UpdateContract(ilex, "/Users/yinwei/work/neo/3/ilex/ilexNft/bin/sc/", "ilex");

            //SetBaseName(ilex, "ilexNFT #");
            //BaseName(ilex);

            //SetBaseImage(ilex, "ipfs://NewUriToReplace/");
            //BatchCreate(ilex, "./metadata.txt");
            //UpdateProperties(ilex, "./metadata.txt");
            Properties(ilex, 1);
            Console.ReadKey();
        }

        private static void DeployContract(string path, string fileName)
        {
            Console.WriteLine("deploy contract.");
            string nefFilePath = path + fileName + ".nef";
            string manifestFilePath = path + fileName + ".manifest.json";

            NefFile nefFile;
            nefFile = File.ReadAllBytes(nefFilePath).AsSerializable<NefFile>();
            var mani = File.ReadAllBytes(manifestFilePath);
            ContractManifest manifest = ContractManifest.Parse(mani);

            ContractClient contractClient = new ContractClient(_rpcClient);
            var tx = contractClient.CreateDeployContractTxAsync(nefFile.ToArray(), manifest, keyPair).Result;

            Console.WriteLine(_rpcClient.SendRawTransactionAsync(tx.ToArray()).Result);

            var contractHash = Neo.SmartContract.Helper.GetContractHash(tx.Sender, nefFile.CheckSum, manifest.Name);
            Console.WriteLine("contract hash:" + contractHash);

            Console.WriteLine($"Transaction {tx.Hash} is broadcasted!");
        }

        private static void UpdateContract(UInt160 contract, string path, string fileName)
        {
            Console.WriteLine("update contract.");

            string nefFilePath = path + fileName + ".nef";
            string manifestFilePath = path + fileName + ".manifest.json";

            NefFile nefFile;
            nefFile = File.ReadAllBytes(nefFilePath).AsSerializable<NefFile>();
            var mani = File.ReadAllBytes(manifestFilePath);
            ContractManifest manifest = ContractManifest.Parse(mani);

            byte[] script;
            using (ScriptBuilder sb = new ScriptBuilder())
            {
                sb.EmitDynamicCall(contract, "update", nefFile.ToArray(), manifest.ToJson().ToString(), keyPair.GetScriptHash());
                script = sb.ToArray();
            }

            Signer[] signers = new[] { new Signer { Scopes = WitnessScope.Global, Account = keyPair.GetScriptHash() } };

            Helper.SignAndSendTx(_rpcClient, script, signers, null, keyPair);
        }

        private static void SetBaseName(UInt160 contract, string baseName)
        {
            byte[] script;
            using (ScriptBuilder sb = new ScriptBuilder())
            {
                sb.EmitDynamicCall(contract, "setBaseName", baseName);
                script = sb.ToArray();
            }
            Signer[] signers = new[] { new Signer { Scopes = WitnessScope.Global, Account = keyPair.GetScriptHash() } };

            Helper.SignAndSendTx(_rpcClient, script, signers, null, keyPair);
        }

        private static void SetBaseImage(UInt160 contract, string baseImage)
        {
            byte[] script;
            using (ScriptBuilder sb = new ScriptBuilder())
            {
                sb.EmitDynamicCall(contract, "setBaseImage", baseImage);
                script = sb.ToArray();
            }
            Signer[] signers = new[] { new Signer { Scopes = WitnessScope.Global, Account = keyPair.GetScriptHash() } };

            Helper.SignAndSendTx(_rpcClient, script, signers, null, keyPair);
        }

        private static void BatchCreate(UInt160 contract, string path)
        {
            //UInt160 owner, ByteString tokenId, string name, string description, string image, string dna, BigInteger edition, BigInteger date, string complier, ByteString[] attributes
            byte[] script;
            var txt = File.ReadAllText(path);
            JArray jarray = JArray.Parse(txt);
            decimal count = Math.Ceiling((decimal)jarray.Count / 100);
            for(var i = 0; i < count; i++)
            {
                var ja = jarray.ToArray()[(100 * i)..(100 * (i + 1) > jarray.Count ? jarray.Count : 100 * (i + 1))];
                using (ScriptBuilder sb = new ScriptBuilder())
                {
                    foreach (var jo in ja)
                    {
                        string name = ((string?)jo["name"]);
                        string description = ((string?)jo["description"]);
                        string image = ((string?)jo["image"]);
                        string dna = ((string?)jo["dna"]);
                        int edition = ((int)jo["edition"]);
                        long date = ((long)jo["date"]);
                        string compiler = ((string?)jo["compiler"]);
                        //JArray Ja_atrs = jo["attributes"] as JArray;
                        //ContractParameter contractParameter = new ContractParameter();
                        //contractParameter.Type = ContractParameterType.Array;
                        //contractParameter.Value = new List<ContractParameter>();
                        //foreach (var ja_atr in Ja_atrs)
                        //{
                        //    ContractParameter _contractParameter = new ContractParameter();
                        //    _contractParameter.Type = ContractParameterType.ByteArray;
                        //    ContractParameter trait_type = new ContractParameter(ContractParameterType.String);
                        //    trait_type.SetValue((string?)ja_atr["trait_type"]);
                        //    ContractParameter value = new ContractParameter(ContractParameterType.String);
                        //    value.SetValue((string?)ja_atr["value"]);
                        //    _contractParameter.Value = Convert.FromBase64String(SerilizeAttribute(contract, trait_type, value));
                        //    ((List<ContractParameter>)contractParameter.Value).Add(_contractParameter);
                        //}
                        //string attributes = SerilizeAttributes(contract, contractParameter);
                        //ContractParameter contractParameter1 = new ContractParameter(ContractParameterType.ByteArray);
                        //contractParameter1.Value = Convert.FromBase64String(attributes);
                        sb.EmitDynamicCall(contract, "create", keyPair.GetScriptHash(), dna, edition, date);
                    }
                    script = sb.ToArray();
                }

                Signer[] signers = new[] { new Signer { Scopes = WitnessScope.Global, Account = keyPair.GetScriptHash() } };

                Helper.SignAndSendTx(_rpcClient, script, signers, null, keyPair);
            }
        }


        private static void UpdateProperties(UInt160 contract, string path)
        {
            //UInt160 owner, ByteString tokenId, string name, string description, string image, string dna, BigInteger edition, BigInteger date, string complier, ByteString[] attributes
            byte[] script;
            var txt = File.ReadAllText(path);
            JArray jarray = JArray.Parse(txt);
            using (ScriptBuilder sb = new ScriptBuilder())
            {
                JArray Ja_atrs = jarray[0]["attributes"] as JArray;
                ContractParameter contractParameter = new ContractParameter();
                contractParameter.Type = ContractParameterType.Array;
                contractParameter.Value = new List<ContractParameter>();
                foreach (var ja_atr in Ja_atrs)
                {
                    ContractParameter _contractParameter = new ContractParameter();
                    _contractParameter.Type = ContractParameterType.ByteArray;
                    ContractParameter trait_type = new ContractParameter(ContractParameterType.String);
                    trait_type.SetValue((string?)ja_atr["trait_type"]);
                    ContractParameter value = new ContractParameter(ContractParameterType.String);
                    value.SetValue((string?)ja_atr["value"]);
                    _contractParameter.Value = Convert.FromBase64String(SerilizeAttribute(contract, trait_type, value));
                    ((List<ContractParameter>)contractParameter.Value).Add(_contractParameter);
                }
                string attributes = SerilizeAttributes(contract, contractParameter);
                ContractParameter contractParameter1 = new ContractParameter(ContractParameterType.ByteArray);
                contractParameter1.Value = Convert.FromBase64String(attributes);
                sb.EmitDynamicCall(contract, "updateProperties", 1, contractParameter1);
                script = sb.ToArray();
            }

            Signer[] signers = new[] { new Signer { Scopes = WitnessScope.Global, Account = keyPair.GetScriptHash() } };

            Helper.SignAndSendTx(_rpcClient, script, signers, null, keyPair);
        }

        private static string SerilizeAttribute(UInt160 _ilex, ContractParameter type, ContractParameter value)
        {
            byte[] script;
            using (ScriptBuilder sb = new ScriptBuilder())
            {
                sb.EmitDynamicCall(_ilex, "serilizeAttribute", type, value);
                script = sb.ToArray();
            }
            Signer[] signers = new[] { new Signer { Scopes = WitnessScope.Global, Account = keyPair.GetScriptHash() } };
            return Helper.InvokeScript(_rpcClient, script, signers);
        }

        private static string SerilizeAttributes(UInt160 _ilex, ContractParameter contractParameter)
        {
            byte[] script;
            using (ScriptBuilder sb = new ScriptBuilder())
            {
                sb.EmitDynamicCall(_ilex, "serilizeAttributes", contractParameter);
                script = sb.ToArray();
            }
            Signer[] signers = new[] { new Signer { Scopes = WitnessScope.Global, Account = keyPair.GetScriptHash() } };
            return Helper.InvokeScript(_rpcClient, script, signers);
        }

        private static string DeserializeAttribute(UInt160 _ilex, ContractParameter contractParameter)
        {
            byte[] script;
            using (ScriptBuilder sb = new ScriptBuilder())
            {
                sb.EmitDynamicCall(_ilex, "deserializeAttribute", contractParameter);
                script = sb.ToArray();
            }
            Signer[] signers = new[] { new Signer { Scopes = WitnessScope.Global, Account = keyPair.GetScriptHash() } };
            return Helper.InvokeScript(_rpcClient, script, signers);
        }

        private static string BaseName(UInt160 _ilex)
        {
            byte[] script;
            using (ScriptBuilder sb = new ScriptBuilder())
            {
                sb.EmitDynamicCall(_ilex, "baseName");
                script = sb.ToArray();
            }
            Signer[] signers = new[] { new Signer { Scopes = WitnessScope.Global, Account = keyPair.GetScriptHash() } };
            return Helper.InvokeScript(_rpcClient, script, signers);
        }

        private static string DeserializeAttributes(UInt160 _ilex, ContractParameter contractParameter)
        {
            byte[] script;
            using (ScriptBuilder sb = new ScriptBuilder())
            {
                sb.EmitDynamicCall(_ilex, "deserializeAttributes", contractParameter);
                script = sb.ToArray();
            }
            Signer[] signers = new[] { new Signer { Scopes = WitnessScope.Global, Account = keyPair.GetScriptHash() } };
            return Helper.InvokeScript(_rpcClient, script, signers);
        }

        private static string Properties(UInt160 _ilex, BigInteger tokenid)
        {
            byte[] script;
            using (ScriptBuilder sb = new ScriptBuilder())
            {
                sb.EmitDynamicCall(_ilex, "properties", tokenid);
                script = sb.ToArray();
            }
            Signer[] signers = new[] { new Signer { Scopes = WitnessScope.Global, Account = keyPair.GetScriptHash() } };
            return Helper.InvokeScript(_rpcClient, script, signers);
        }
    }
}
