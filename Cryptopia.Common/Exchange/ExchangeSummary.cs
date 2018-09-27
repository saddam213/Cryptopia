using System.Collections.Generic;

namespace Cryptopia.Common.Exchange
{
	public class ExchangeSummary
	{
		public ExchangeSummary()
		{
			BaseMarket = "BTC";
			Featured = new List<FeaturedCurrency>();
			TopMarkets = new List<ExchangeSummaryModel>();
			BaseCurrencies = new List<ExchangeSummaryBaseCurrency>();
		}

		public int TotalMarkets { get; set; }
		public int TotalTrades { get; set; }
		public List<FeaturedCurrency> Featured { get; set; }
		public List<ExchangeSummaryBaseCurrency> BaseCurrencies { get; set; }
		public List<ExchangeSummaryModel> TopMarkets { get; set; }
		public string BaseMarket { get; set; }
	}
}
