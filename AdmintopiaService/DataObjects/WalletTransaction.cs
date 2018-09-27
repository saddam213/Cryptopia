using System;
using System.Runtime.Serialization;
using Cryptopia.WalletAPI.DataObjects;

namespace AdmintopiaService.DataObjects
{
	[DataContract]
	public class WalletTransaction
	{
		[DataMember]
		public string Account { get; set; }

		[DataMember]
		public TransactionDataType Type { get; set; }

		[DataMember]
		public decimal Amount { get; set; }

		[DataMember]
		public string Txid { get; set; }

		[DataMember]
		public DateTime Timestamp { get; set; }

		[DataMember]
		public string Address { get; set; }

		[DataMember]
		public int Confirmations { get; set; }
	}
}