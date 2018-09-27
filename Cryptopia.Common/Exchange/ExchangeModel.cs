using System.Collections.Generic;
using Cryptopia.Common.TradePair;
using Cryptopia.Common.Currency;
using Cryptopia.Common.Api;

namespace Cryptopia.Common.Exchange
{
	public class ExchangeModel
	{
		public TradePairModel TradePair { get; set; }
		//public List<CurrencyModel> Currencies { get; set; }
		public List<BaseCurrencyModel> BaseCurrencies { get; set; }
		public string BaseMarket { get; set; }
		public ExchangeSummary Summary { get; set; }
	}

	public class ExchangeInfoModel
	{
		public TradePairTickerModel Ticker { get; set; }
		public TradePairModel TradePair { get; set; }
	}
	
}