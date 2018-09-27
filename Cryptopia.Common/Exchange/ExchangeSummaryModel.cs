namespace Cryptopia.Common.Exchange
{
	public class ExchangeSummaryModel
	{
		public int TradePairId { get; set; }
		public int TotalTrades { get; set; }
		public string Symbol { get; set; }
		public string BaseSymbol { get; set; }
		public decimal Low { get; set; }
		public decimal High { get; set; }
		public decimal Volume { get; set; }
		public double Change { get; set; }
		public decimal TotalBase { get; set; }
	}
}
