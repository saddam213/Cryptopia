using System;
using System.Runtime.Serialization;

namespace Cryptopia.Common.Api
{
	[DataContract]
	public class ApiOpenOrder
	{
		[DataMember(Order = 0)]
		public int OrderId { get; set; }

		[DataMember(Order = 1)]
		public int TradePairId { get; set; }

		[DataMember(Order = 2)]
		public string Market { get; set; }

		[DataMember(Order = 3)]
		public string Type { get; set; }

		[DataMember(Order = 4)]
		public decimal Rate { get; set; }

		[DataMember(Order = 5)]
		public decimal Amount { get; set; }

		[DataMember(Order = 6)]
		public decimal Total
		{
			get { return decimal.Round(Amount * Rate, 8); }
		}

		[DataMember(Order = 7)]
		public decimal Remaining { get; set; }

		[DataMember(Order = 8)]
		public DateTime TimeStamp { get; set; }
	}
}