using System;

namespace Cryptopia.Common.Api
{

	public class ApiUserTradeHistoryRequest
	{
		/// <summary>
		/// Gets or sets the user identifier.
		/// </summary>

		public Guid UserId { get; set; }

		/// <summary>
		/// Gets or sets the api request string.
		/// </summary>

		public string Request { get; set; }


		public int? TradePairId { get; set; }


		public string Market { get; set; }


		public int? Count { get; set; }
	}
}