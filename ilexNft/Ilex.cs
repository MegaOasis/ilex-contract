using System;
using System.ComponentModel;
using Neo;
using Neo.SmartContract;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;
using System.Numerics;
using Neo.SmartContract.Framework.Native;
using System.Collections.Generic;
using Neo.SmartContract.Framework;

namespace ilexNft
{
    //string dna, BigInteger edition, BigInteger date
    public struct PaymentData17
    {
        public string Dna;
        public BigInteger Edition;
        public BigInteger Date;
    }

    public struct IlexAttribute
    {
        public string Trait_type;
        public string Value;
    }

    public class TokenState : Nep11TokenState
    {
        public BigInteger TokenId;
        public string Dna;
        public BigInteger Edition;
        public BigInteger Date;
        public ByteString Attributes;

    }

    [DisplayName("ilex")]
    [ManifestExtra("Author", "")]
    [ManifestExtra("Email", "")]
    [ManifestExtra("Description", "")]
    [ContractPermission("*", "*")]
    public partial class Ilex : Nep11Token<TokenState>
    {
        [DisplayName("UpdateProperties")]
        public static event Action<string> OnUpdateProperties;

        [Safe]
        public override string Symbol()
        {
            return "ilex";
        }

        [Safe]
        public static string Version()
        {
            return "20220629";
        }

        [Safe]
        public static string Description()
        {
            return "ilex test snake";
        }

        [Safe]
        public static string Compiler()
        {
            return "ilex";
        }

        [Safe]
        public static string BaseName()
        {
            return BaseNameStorage.Get();
        }

        [Safe]
        public static string BaseImage()
        {
            return BaseImageStorage.Get();
        }

        [Safe]
        public static BigInteger FloorPrice(UInt160 asset)
        {
            return AssetStorage.Get(asset);
        }

        [Safe]
        public static ByteString SerilizeNEP17Payment(string dna, BigInteger edition, BigInteger date)
        {
            PaymentData17 paymentData17 = new PaymentData17() { Date = date, Edition = edition, Dna = dna };
            return StdLib.Serialize(paymentData17);
        }

        [Safe]
        public static ByteString SerilizeAttribute(ByteString type, ByteString value)
        {
            IlexAttribute attribute = new IlexAttribute() { Trait_type = type, Value = value };

            return StdLib.Serialize(attribute);
        }

        [Safe]
        public static Map<string, string> DeserializeAttribute(ByteString str)
        {
            IlexAttribute attr = (IlexAttribute)StdLib.Deserialize(str);
            Map<string, string> map = new();
            map["trait_type"] = attr.Trait_type;
            map["value"] = attr.Value;
            return map;
        }

        [Safe]
        public static ByteString SerilizeAttributes(ByteString[] attrs)
        {
            return StdLib.Serialize(attrs);
        }

        [Safe]
        public static Map<string, string>[] DeserializeAttributes(ByteString bytestring)
        {
            ByteString[] strs = (ByteString[])StdLib.Deserialize(bytestring);
            Map<string, string>[] maps = new Map<string, string>[strs.Length];
            for (var i = 0; i < strs.Length; i++)
            {
                var str = strs[i];
                IlexAttribute attr = (IlexAttribute)StdLib.Deserialize(str);
                Map<string, string> map = new Map<string, string>();
                map["trait_type"] = attr.Trait_type;
                map["value"] = attr.Value;
                maps[i] = map;
            }
            return maps;
        }

        [Safe]
        public override Map<string, object> Properties(ByteString tokenId)
        {
            StorageMap tokenMap = new(Storage.CurrentContext, Prefix_Token);
            TokenState token = (TokenState)StdLib.Deserialize(tokenMap[tokenId]);
            Map<string, object> map = new();
            map["name"] = token.Name;
            map["id"] = token.TokenId;
            map["description"] = Description();
            map["image"] = BaseImage() + (BigInteger)tokenId + ".png";
            map["dna"] = token.Dna;
            map["edition"] = token.Edition;
            map["date"] = token.Date;
            map["compiler"] = Compiler();
            if(token.Attributes != "")
            {
                Map<string, string>[] attributes = DeserializeAttributes(token.Attributes);
                map["attributes"] = attributes;
            }
            else
            {
                map["attributes"] = new Array[0];
            }
            return map;
        }

        public static void OnNEP17Payment(UInt160 from, BigInteger amount, object data)
        {
            UInt160 caller = Runtime.CallingScriptHash;
            Assert(amount > 0, "OnNEP17Payment: amount need more than zero");
            Assert(AssetStorage.Get(caller) == amount, "OnNEP17Payment: amount");
            PaymentData17 paymentData17 = (PaymentData17)StdLib.Deserialize((ByteString)data);
            Create(from, paymentData17.Dna, paymentData17.Edition, paymentData17.Date);
        }

        public static void OnNEP11Payment(UInt160 from, BigInteger amount, ByteString tokenId, object data)
        {

        }


        private static void Create(UInt160 owner, string dna, BigInteger edition, BigInteger date)
        {
            CounterStorage.Increase();
            BigInteger tokenId = CounterStorage.Current();

            StorageMap tokenMap = new(Storage.CurrentContext, Prefix_Token);
            var data = tokenMap.Get((ByteString)tokenId);
            Assert(data is null, "Create: tokenid exist");
            TokenState tokenState = new TokenState()
            {
                TokenId = tokenId,
                Name = BaseName() + tokenId,
                Dna = dna,
                Owner = owner,
                Edition = edition,
                Date = date,
                Attributes = ""
            };
            Mint((ByteString)tokenId, tokenState);
        }
    }
}

