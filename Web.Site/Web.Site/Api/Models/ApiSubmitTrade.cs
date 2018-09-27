using Cryptopia.Enums;

namespace Web.Site.Api.Models
{
	public class ApiSubmitTrade
	{
		public int? TradePairId { get; set; }
		public string Market { get; set; }
		public TradeHistoryType Type { get; set; }
		public decimal Rate { get; set; }
		public decimal Amount { get; set; }
	}
}