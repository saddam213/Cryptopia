using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Cryptopia.WalletAPI.DataObjects
{
	/// <summary>
	/// Transaction rpc data response object
	/// </summary>
	[DataContract]
    public class TransactionData
    {
        /// <summary>
        /// Gets or sets the account.
        /// </summary>
        [DataMember(Name = "account")]
        public string Account { get; set; }

        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        [DataMember(Name = "address")]
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets the moved to account.
        /// </summary>
        [DataMember(Name = "otheraccount")]
        public string MovedToAccount { get; set; }

        /// <summary>
        /// Gets or sets the category.
        /// </summary>
        [DataMember(Name = "category")]
        public string Category { get; set; }

        /// <summary>
        /// Gets or sets the amount.
        /// </summary>
        [DataMember(Name = "amount")]
        public decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets the confirmations.
        /// </summary>
        [DataMember(Name = "confirmations")]
        public int Confirmations { get; set; }

        /// <summary>
        /// Gets or sets the blockhash.
        /// </summary>
        [DataMember(Name = "blockhash")]
        public string Blockhash { get; set; }

        /// <summary>
        /// Gets or sets the blockindex.
        /// </summary>
        [DataMember(Name = "blockindex")]
        public int Blockindex { get; set; }

        /// <summary>
        /// Gets or sets the txid.
        /// </summary>
        [DataMember(Name = "txid")]
        public string Txid { get; set; }

        /// <summary>
        /// Gets or sets the time.
        /// </summary>
        [DataMember(Name = "time")]
        public uint Time { get; set; }
    }



    public class ScriptSig
    {
        public string asm { get; set; }
        public string hex { get; set; }
    }

    public class Vin
    {
        public string txid { get; set; }
        public int vout { get; set; }
        public ScriptSig scriptSig { get; set; }
        public long sequence { get; set; }
        public string coinbase { get; set; }
    }

    public class ScriptPubKey
    {
        public string asm { get; set; }
        public string hex { get; set; }
        public int reqSigs { get; set; }
        public string type { get; set; }
        public List<string> addresses { get; set; }
    }

    public class Vout
    {
        public decimal value { get; set; }
        public int n { get; set; }
        public ScriptPubKey scriptPubKey { get; set; }
    }

    public class TransactionRawData
    {
        public string hex { get; set; }
        public string txid { get; set; }
        public int version { get; set; }
        public int locktime { get; set; }
        public List<Vin> vin { get; set; }
        public List<Vout> vout { get; set; }
        public string blockhash { get; set; }
        public int confirmations { get; set; }
        public uint time { get; set; }
        public uint blocktime { get; set; }
    }


}
