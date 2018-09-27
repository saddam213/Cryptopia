using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Cryptopia.WalletAPI.DataObjects
{
	/// <summary>
	/// ListSinceBlockData rpc data response object
	/// </summary>
	[DataContract]
    public class ListSinceBlockData
    {
        /// <summary>
        /// Gets or sets the transactions.
        /// </summary>
        [DataMember(Name = "transactions")]
        public List<TransactionData> Transactions { get; set; }
    }
}
