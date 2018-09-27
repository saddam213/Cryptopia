

using Cryptopia.Enums;
using System;

namespace Cryptopia.Common.Api
{

	public class ApiSubmitUserTradeRequest
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


		public decimal Amount { get; set; }


		public decimal Rate { get; set; }


		public TradeHistoryType Type { get; set; }
	}
}