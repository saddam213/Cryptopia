using System.Runtime.Serialization;

namespace Cryptopia.Common.Api
{
	[DataContract]
	public class ApiMarketHistory
	{
		/// <summary>
		/// Gets or sets the trade pair identifier.
		/// </summary>
		[DataMember(Order = 0)]
		public int TradePairId { get; set; }

		/// <summary>
		/// Gets or sets the label.
		/// </summary>
		[DataMember(Order = 1)]
		public string Label { get; set; }

		/// <summary>
		/// Gets or sets the type.
		/// </summary>
		[DataMember(Order = 2)]
		public string Type { get; set; }

		/// <summary>
		/// Gets or sets the price.
		/// </summary>
		[DataMember(Order = 3)]
		public decimal Price { get; set; }

		/// <summary>
		/// Gets or sets the amount.
		/// </summary>
		[DataMember(Order = 4)]
		public decimal Amount { get; set; }

		/// <summary>
		/// Gets or sets the total.
		/// </summary>
		[DataMember(Order = 5)]
		public decimal Total { get; set; }

		/// <summary>
		/// Gets or sets the timestamp.
		/// </summary>
		[DataMember(Order = 6)]
		public int Timestamp { get; set; }
	}
}