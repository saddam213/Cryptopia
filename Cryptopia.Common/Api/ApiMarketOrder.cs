using System.Runtime.Serialization;

namespace Cryptopia.Common.Api
{
	[DataContract]
	public class ApiMarketOrder
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
		/// Gets or sets the price.
		/// </summary>
		[DataMember(Order = 2)]
		public decimal Price { get; set; }

		/// <summary>
		/// Gets or sets the volume.
		/// </summary>
		[DataMember(Order = 3)]
		public decimal Volume { get; set; }

		/// <summary>
		/// Gets or sets the total.
		/// </summary>
		[DataMember(Order = 4)]
		public decimal Total { get; set; }
	}
}