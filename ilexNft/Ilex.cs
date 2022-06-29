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
    public struct IlexAttribute
    {
        public string Trait_type;
        public string Value;
    }

    public class TokenState : Nep11TokenState
    {
        public string Description;
        public string Image;
        public string Dna;
        public BigInteger Edition;
        public BigInteger Date;
        public ByteString Attributes;
        public string Compiler;

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
            return "MetaPanacea";
        }

        [Safe]
        public string Version()
        {
            return "20220628";
        }

        [Safe]
        public ByteString SerilizeAttribute(ByteString type, ByteString value)
        {
            IlexAttribute attribute = new IlexAttribute() { Trait_type = type, Value = value };

            return StdLib.Serialize(attribute);
        }

        [Safe]
        public Map<string, string> DeserializeAttribute(ByteString str)
        {
            IlexAttribute attr = (IlexAttribute)StdLib.Deserialize(str);
            Map<string, string> map = new();
            map["trait_type"] = attr.Trait_type;
            map["value"] = attr.Value;
            return map;
        }

        [Safe]
        public ByteString SerilizeAttributes(ByteString[] attrs)
        {
            return StdLib.Serialize(attrs);
        }

        [Safe]
        public Map<string, string>[] DeserializeAttributes(ByteString bytestring)
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
            map["description"] = token.Description;
            map["image"] = token.Image;
            map["dna"] = token.Dna;
            map["edition"] = token.Edition;
            map["date"] = token.Date;
            map["compiler"] = token.Compiler;
            Map<string, string>[] attributes = DeserializeAttributes(token.Attributes);
            map["attributes"] = attributes;
            return map;
        }

        public static void Create(UInt160 owner, ByteString tokenId, string name, string description, string image, string dna, BigInteger edition, BigInteger date, string complier, ByteString attributes)
        {
            StorageMap tokenMap = new(Storage.CurrentContext, Prefix_Token);
            var data = tokenMap.Get(tokenId);
            Assert(data is null, "Create: tokenid exist");
            Assert(Runtime.CheckWitness(GetOwner()), "Create: CheckWitness failed");
            TokenState tokenState = new TokenState() { Name = name, Description = description, Image = image, Dna = dna, Owner = owner, Edition = edition, Date = date, Compiler = complier, Attributes = attributes};
            Mint(tokenId, tokenState);
        }

        public static void UpdateProperties(UInt160 owner, ByteString tokenId, string name, string description, string image, string dna, BigInteger edition, BigInteger date, string complier, ByteString attributes)
        {
            Assert(Runtime.CheckWitness(GetOwner()), "Create: CheckWitness failed");
            TokenState tokenState = new TokenState() { Name = name, Description = description, Image = image, Dna = dna, Owner = owner, Edition = edition, Date = date, Compiler = complier, Attributes = attributes};
            StorageMap tokenMap = new(Storage.CurrentContext, Prefix_Token);
            tokenMap.Put(tokenId, StdLib.Serialize(tokenMap));
            OnUpdateProperties(tokenId);
        }
    }
}

