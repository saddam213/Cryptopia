using System;
using Cryptopia.WalletAPI.DataObjects;

namespace Cryptopia.WalletAPI.Helpers
{
    /// <summary>
    /// Extension methods
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// To the transaction string.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static string ToTransactionString(this TransactionDataType type)
        {
            switch (type)
            {
                case TransactionDataType.All:
                    return string.Empty;
                case TransactionDataType.Deposit:
                   return "receive";
                case TransactionDataType.Withdraw:
                    return "send";
                case TransactionDataType.Transfer:
                    return "move";
                case TransactionDataType.Mined:
                    return "generate";
				case TransactionDataType.Savings:
					return "savings";
                default:
                    break;
            }
            return string.Empty;
        }

        public static TransactionDataType ToTransactionType(string type)
        {
            switch (type)
            {
                case "receive":
                    return TransactionDataType.Deposit;
                case "send":
                    return TransactionDataType.Withdraw;
                case "generate":
                    return TransactionDataType.Mined;
                case "immature":
                    return TransactionDataType.Immature;
                case "orphan":
                    return TransactionDataType.Orphan;
				case "savings":
					return TransactionDataType.Savings;
                default:
                    break;
            }
            return TransactionDataType.All;
        }

        ///// <summary>
        ///// Converts UNIX timestamp to DateTime object.
        ///// </summary>
        ///// <param name="time">The time.</param>
        ///// <returns>the time, LOL</returns>
        //public static DateTime ConvertToDateTime(this uint time)
        //{
        //    return new DateTime(1970, 1, 1).AddSeconds(time);
        //}
    }
}
