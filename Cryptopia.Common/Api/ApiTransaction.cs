using System;
using System.Runtime.Serialization;

namespace Cryptopia.Common.Api
{
	[DataContract]
	public class ApiTransactionResult
	{
		[DataMember(Order = 0)]
		public int Id { get; set; }

		[DataMember(Order = 1)]
		public string Currency { get; set; }

		[DataMember(Order = 2)]
		public string TxId { get; set; }

		[DataMember(Order = 3)]
		public string Type { get; set; }

		[DataMember(Order = 4)]
		public decimal Amount { get; set; }

		[DataMember(Order = 5)]
		public decimal Fee { get; set; }

		[DataMember(Order = 6)]
		public string Status { get; set; }

		[DataMember(Order = 7)]
		public int Confirmations { get; set; }

		[DataMember(Order = 8)]
		public DateTime Timestamp { get; set; }

		[DataMember(Order = 9)]
		public string Address { get; set; }
	}
}