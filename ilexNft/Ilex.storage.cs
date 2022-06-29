using System;
using Neo;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services;

namespace ilexNft
{
    partial class Ilex
    {
        private static readonly byte[] ownerPrefix = new byte[] { 0x01, 0x01 };

        public static class OwnerStorage
        {
            internal static void Put(UInt160 usr)
            {
                StorageMap map = new(Storage.CurrentContext, ownerPrefix);
                map.Put((ByteString)"owner", usr);
            }

            internal static UInt160 Get()
            {
                StorageMap map = new(Storage.CurrentReadOnlyContext, ownerPrefix);
                byte[] v = (byte[])map.Get((ByteString)"owner");
                if (v is null)
                {
                    return InitialOwner;
                }
                else if (v.Length != 20)
                {
                    return InitialOwner;
                }
                else
                {
                    return (UInt160)v;
                }
            }

            internal static void Delete()
            {
                StorageMap map = new(Storage.CurrentContext, ownerPrefix);
                map.Delete((ByteString)"owner");
            }
        }
    }
}

