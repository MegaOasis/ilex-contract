using System;
using Neo;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services;
using System.Numerics;

namespace ilexNft
{
    partial class Ilex
    {
        private static readonly byte[] ownerPrefix = new byte[] { 0x01, 0x01 };

        private static readonly byte[] counterPrefix = new byte[] { 0x01, 0x02 };

        private static readonly byte[] baseNamePrefix = new byte[] { 0x01, 0x03 };

        private static readonly byte[] baseImagePrefix = new byte[] { 0x01, 0x04 };

        private static readonly byte[] assetPrefix = new byte[] { 0x01, 0x05 };

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

        public static class CounterStorage
        {
            internal static void Put(BigInteger count)
            {
                StorageMap map = new(Storage.CurrentContext, counterPrefix);
                map.Put((ByteString)"counter", count);
            }

            internal static BigInteger Current()
            {
                StorageMap map = new(Storage.CurrentContext, counterPrefix);
                return (BigInteger)map.Get((ByteString)"counter");
            }

            internal static void Increase()
            {
                Put(Current() + 1);
            }
        }

        public static class BaseNameStorage
        {
            internal static void Put(string baseName)
            {
                StorageMap map = new(Storage.CurrentContext, baseNamePrefix);
                map.Put((ByteString)"baseName", baseName);
            }

            internal static string Get()
            {
                StorageMap map = new(Storage.CurrentContext, baseNamePrefix);
                return (string)map.Get((ByteString)"baseName");
            }
        }

        public static class BaseImageStorage
        {
            internal static void Put(string baseImage)
            {
                StorageMap map = new(Storage.CurrentContext, baseImagePrefix);
                map.Put((ByteString)"baseImage", baseImage);
            }

            internal static string Get()
            {
                StorageMap map = new(Storage.CurrentContext, baseImagePrefix);
                return (string)map.Get((ByteString)"baseImage");
            }
        }

        public static class AssetStorage
        {
            internal static void Put(UInt160 asset, BigInteger price)
            {
                StorageMap map = new(Storage.CurrentContext, assetPrefix);
                map.Put(asset, price);
            }

            internal static BigInteger Get(UInt160 asset)
            {
                StorageMap map = new(Storage.CurrentContext, assetPrefix);
                var data = map.Get(asset);
                if (data is not null)
                    return (BigInteger)map.Get(asset);
                else
                    return 0;
            }

        }
    }
}

