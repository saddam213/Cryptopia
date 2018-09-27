using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Cryptopia.Common.Api
{
	[DataContract]
	public class ApiMarketOrderData
	{
		/// <summary>
		/// Gets or sets the buy.
		/// </summary>
		[DataMember(Order = 0)]
		public List<ApiMarketOrder> Buy { get; set; }

		/// <summary>
		/// Gets or sets the sell.
		/// </summary>
		[DataMember(Order = 1)]
		public List<ApiMarketOrder> Sell { get; set; }
	}
}