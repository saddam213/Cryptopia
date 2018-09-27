using Cryptopia.Common.Arbitrage;
using Cryptopia.Common.Currency;
using System.Collections.Generic;


namespace Web.Site.Models
{
	public class ArbitrageMarketModel
	{
		public ArbitrageMarketModel()
		{
			Exchanges = new List<string>();
			Markets = new Dictionary<CurrencyModel, List<ArbitrageDataModel>>();
		}

		public List<string> Exchanges { get; set; }

		public Dictionary<CurrencyModel, List<ArbitrageDataModel>> Markets { get; set; }
	}
}