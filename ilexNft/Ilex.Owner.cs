using System;
using System.ComponentModel;
using Neo;
using Neo.SmartContract;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;

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


        /// <summary>
        /// 更换合约所有者
        /// </summary>
        /// <param name="owner"></param>
        /// <returns></returns>
        public static bool ChangeOwner(UInt160 owner)
        {
            Assert(Runtime.CheckWitness(GetOwner()), "SetOwner: CheckWitness failed, owner-".ToByteArray().Concat(owner).ToByteString());
            Assert(CheckAddrVaild(owner), "SetOwner: invalid owner-".ToByteArray().Concat(owner).ToByteString());
            OwnerStorage.Put(owner);
            OnChangeOwner(owner);
            return true;
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

