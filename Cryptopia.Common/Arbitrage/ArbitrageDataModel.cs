namespace Cryptopia.Common.Arbitrage
{
	public class ArbitrageDataModel
	{
		public int TradePairId { get; set; }
		public string Exchange { get; set; }
		public string Currency { get; set; }
		public int CurrencyId { get; set; }
		public int BaseCurrencyId { get; set; }
		public string Symbol { get; set; }
		public string BaseSymbol { get; set; }
		public decimal Ask { get; set; }
		public decimal Bid { get; set; }
		public decimal Last { get; set; }
		public decimal Volume { get; set; }
		public decimal BaseVolume { get; set; }
		public string MarketUrl { get; set; }
	}
}
