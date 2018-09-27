using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Cryptopia.Common.Api
{
	[DataContract]
	public class ApiUserOpenOrdersResponse
	{
		[DataMember(Order = 0)]
		public bool Success { get; set; }

		[DataMember(Order = 1)]
		public string Error { get; set; }

		[DataMember(Order = 2)]
		public List<ApiOpenOrder> Data { get; set; }
	}
}