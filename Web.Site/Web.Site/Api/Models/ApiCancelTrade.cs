using Cryptopia.Enums;

namespace Web.Site.Api.Models
{
	public class ApiCancelTrade
	{
		public CancelTradeType Type { get; set; }
		public int? OrderId { get; set; }
		public int? TradePairId { get; set; }
	}

}