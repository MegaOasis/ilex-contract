using System;
using System.ComponentModel;
using Neo;
using Neo.SmartContract.Framework;

namespace ilexNft
{
    partial class Ilex
    {
        /// <summary>
        /// params: message, extend data
        /// </summary>
        [DisplayName("Fault")]
        public static event FaultEvent onFault;
        public delegate void FaultEvent(string message, params object[] paras);

        /// <summary>
        /// 断言
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="message"></param>
        private static void Assert(bool condition, string message)
        {
            if (!condition)
            {
                onFault(message, null);
                ExecutionEngine.Assert(false);
            }
        }

        private static bool CheckAddrVaild(params UInt160[] addrs)
        {
            bool vaild = true;

            foreach (UInt160 addr in addrs)
            {
                vaild = vaild && addr is not null && addr.IsValid;
                if (!vaild)
                    break;
            }

            return vaild;
        }
    }
}

