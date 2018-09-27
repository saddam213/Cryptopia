using System.Collections.Generic;
using System.Runtime.Serialization;
using Cryptopia.Common.Trade;

namespace Cryptopia.Common.Api
{
	[DataContract]
	public class ApiSubmitUserTradeResponse
	{
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="ApiCurrencyResult"/> is success.
		/// </summary>
		/// <value>
		///   <c>true</c> if success; otherwise, <c>false</c>.
		/// </value>
		[DataMember(Order = 0)]
		public bool Success { get; set; }

		[DataMember(Order = 1)]
		public string Error { get; set; }

		[DataMember(Order = 2)]
		public ApiSubmitUserTradeData Data { get; set; }
	}

	[DataContract]
	public class ApiSubmitUserTradeData
	{
		[DataMember(Order = 0)]
		public int? OrderId { get; set; }

		[DataMember(Order = 1)]
		public List<int> FilledOrders { get; set; }
	}
}