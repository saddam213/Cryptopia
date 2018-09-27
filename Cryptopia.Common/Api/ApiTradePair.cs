using System.Runtime.Serialization;

namespace Cryptopia.Common.Api
{
	[DataContract]
	public class ApiTradePair
	{
		public ApiTradePair()
		{
			MaximumPrice = 100000000000;
			MinimumPrice = 0.00000001m;
		}

		/// <summary>
		/// Gets or sets the identifier.
		/// </summary>
		[DataMember(Order = 0)]
		public int Id { get; set; }

		/// <summary>
		/// Gets or sets the label.
		/// </summary>
		[DataMember(Order = 1)]
		public string Label { get; set; }

		/// <summary>
		/// Gets or sets the currency.
		/// </summary>
		[DataMember(Order = 2)]
		public string Currency { get; set; }

		/// <summary>
		/// Gets or sets the symbol.
		/// </summary>
		[DataMember(Order = 3)]
		public string Symbol { get; set; }

		/// <summary>
		/// Gets or sets the base currency.
		/// </summary>
		[DataMember(Order = 4)]
		public string BaseCurrency { get; set; }

		/// <summary>
		/// Gets or sets the base symbol.
		/// </summary>
		[DataMember(Order = 5)]
		public string BaseSymbol { get; set; }

		[DataMember(Order = 6)]
		public string Status { get; set; }

		[DataMember(Order = 7)]
		public string StatusMessage { get; set; }

		[DataMember(Order = 8)]
		public decimal TradeFee { get; set; }

		[DataMember(Order = 9)]
		public decimal MinimumTrade { get; set; }

		[DataMember(Order = 10)]
		public decimal MaximumTrade { get; set; }

		[DataMember(Order = 11)]
		public decimal MinimumBaseTrade { get; set; }

		[DataMember(Order = 12)]
		public decimal MaximumBaseTrade { get; set; }

		[DataMember(Order = 13)]
		public decimal MinimumPrice { get; set; }

		[DataMember(Order = 14)]
		public decimal MaximumPrice { get; set; }

	}
}