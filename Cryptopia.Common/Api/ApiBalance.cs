using System.Runtime.Serialization;

namespace Cryptopia.Common.Api
{
	[DataContract]
	public class ApiBalance
	{
		[DataMember(Order = 0)]
		public int CurrencyId { get; set; }

		[DataMember(Order = 1)]
		public string Symbol { get; set; }

		[DataMember(Order = 2)]
		public decimal Total { get; set; }

		[DataMember(Order = 3)]
		public decimal Available { get; set; }

		[DataMember(Order = 4)]
		public decimal Unconfirmed { get; set; }

		[DataMember(Order = 5)]
		public decimal HeldForTrades { get; set; }

		[DataMember(Order = 6)]
		public decimal PendingWithdraw { get; set; }

		[DataMember(Order = 7)]
		public string Address { get; set; }

		[DataMember(Order = 8)]
		public string Status { get; set; }

		[DataMember(Order = 9)]
		public string StatusMessage { get; set; }

		[DataMember(Order = 10)]
		public string BaseAddress { get; set; }
	}
}