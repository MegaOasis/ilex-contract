using System;
using System.ComponentModel;
using Neo;
using Neo.SmartContract;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;
using System.Numerics;
using Neo.SmartContract.Framework.Native;

namespace ilexNft
{
    partial class Ilex
    {
        [InitialValue("NbtRKN9wPV7WMAQFhVGsecDfNcHZcWkKoY", ContractParameterType.Hash160)]
        private static readonly UInt160 InitialOwner;

        [DisplayName("ChangeOwner")]
        public static event Action<UInt160> OnChangeOwner;

        public static bool Verify() => IsOwner();

        private static bool IsOwner() => Runtime.CheckWitness(GetOwner());

        [Safe]
        public static UInt160 GetOwner()
        {
            return OwnerStorage.Get();
        }

        public static void SetBaseName(string baseName)
        {
            Assert(Runtime.CheckWitness(GetOwner()), "SetBaseName: CheckWitness failed");
            BaseNameStorage.Put(baseName);
        }

        public static void SetBaseImage(string baseImage)
        {
            Assert(Runtime.CheckWitness(GetOwner()), "SetBaseImage: CheckWitness failed");
            BaseImageStorage.Put(baseImage);
        }

        public static void SetAsset(UInt160 asset, BigInteger price)
        {
            Assert(Runtime.CheckWitness(GetOwner()), "SetAsset: CheckWitness failed");
            AssetStorage.Put(asset, price);
        }

        /// <summary>
        /// 更换合约所有者
        /// </summary>
        /// <param name="owner"></param>
        /// <returns></returns>
        public static bool ChangeOwner(UInt160 owner)
        {
            Assert(Runtime.CheckWitness(GetOwner()), "ChangeOwner: CheckWitness failed");
            Assert(CheckAddrVaild(owner), "ChangeOwner: invalid owner");
            OwnerStorage.Put(owner);
            OnChangeOwner(owner);
            return true;
        }

        /// <summary>
        /// 修改nft的属性
        /// </summary>
        /// <param name="tokenId"></param>
        /// <param name="attributes"></param>
        public static void UpdateProperties(BigInteger tokenId, ByteString attributes)
        {
            Assert(Runtime.CheckWitness(GetOwner()), "UpdateProperties: CheckWitness failed");
            StorageMap tokenMap = new(Storage.CurrentContext, Prefix_Token);
            var data = tokenMap.Get((ByteString)tokenId);
            Assert(data is not null, "UpdateProperties: tokenid not exist");
            TokenState tokenState = (TokenState)StdLib.Deserialize(data);
            tokenState.Attributes = attributes;
            tokenMap.Put((ByteString)tokenId, StdLib.Serialize(tokenState));
            OnUpdateProperties((ByteString)tokenId);
        }

        /// <summary>
        /// 升级
        /// </summary>
        /// <param name="nefFile"></param>
        /// <param name="manifest"></param>
        /// <param name="data"></param>
        public static void Update(ByteString nefFile, string manifest, object data)
        {
            Assert(Runtime.CheckWitness(GetOwner()), "No authorization.");
            Neo.SmartContract.Framework.Native.ContractManagement.Update(nefFile, manifest, data);
        }
    }
}

