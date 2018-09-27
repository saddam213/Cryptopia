using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Cryptopia.Common.Api
{
	[DataContract]
	public class ApiMarketOrderGroupData
	{
		[DataMember(Order = 0)]
		public int TradePairId { get; set; }

		[DataMember(Order = 1)]
		public string Market { get; set; }

		[DataMember(Order = 2)]
		public List<ApiMarketOrder> Buy { get; set; }

		[DataMember(Order = 3)]
		public List<ApiMarketOrder> Sell { get; set; }
	}
}