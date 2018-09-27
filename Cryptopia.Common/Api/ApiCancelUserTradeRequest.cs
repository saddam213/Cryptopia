using Cryptopia.Enums;
using System;

namespace Cryptopia.Common.Api
{

	public class ApiCancelUserTradeRequest
	{
		/// <summary>
		/// Gets or sets the user identifier.
		/// </summary>

		public Guid UserId { get; set; }

		/// <summary>
		/// Gets or sets the api request string.
		/// </summary>

		public string Request { get; set; }


		public int? TradeId { get; set; }


		public CancelTradeType CancelType { get; set; }


		public int? TradePairId { get; set; }
	}
}