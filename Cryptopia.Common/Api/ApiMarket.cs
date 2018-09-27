using System.Runtime.Serialization;

namespace Cryptopia.Common.Api
{
	[DataContract]
	public class ApiMarket
	{
		[DataMember(Order = 0)]
		public int TradePairId { get; set; }

		[DataMember(Order = 1)]
		public string Label { get; set; }

		[DataMember(Order = 2)]
		public decimal AskPrice { get; set; }

		[DataMember(Order = 3)]
		public decimal BidPrice { get; set; }

		[DataMember(Order = 4)]
		public decimal Low { get; set; }

		[DataMember(Order = 5)]
		public decimal High { get; set; }

		[DataMember(Order = 6)]
		public decimal Volume { get; set; }

		[DataMember(Order = 7)]
		public decimal LastPrice { get; set; }

		[DataMember(Order = 8)]
		public decimal BuyVolume { get; set; }

		[DataMember(Order = 9)]
		public decimal SellVolume { get; set; }

		[DataMember(Order = 10)]
		public decimal Change { get; set; }


		[DataMember(Order = 11)]
		public decimal Open { get; set; }

		[DataMember(Order = 12)]
		public decimal Close { get; set; }

		[DataMember(Order = 13)]
		public decimal BaseVolume { get; set; }

		[DataMember(Order = 14)]
		public decimal BuyBaseVolume { get; set; }

		[DataMember(Order = 15)]
		public decimal SellBaseVolume { get; set; }
	}
}