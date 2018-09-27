using Cryptopia.Common.Currency;
using System.Collections.Generic;


namespace Web.Site.Models
{
	public class ArbitrageModel
	{
		public ArbitrageModel()
		{
			Exchanges = new List<string>();
			BaseCurrencies = new List<BaseCurrencyModel>();
		}

		public List<string> Exchanges { get; set; }
		public List<BaseCurrencyModel> BaseCurrencies { get; set; }
	}
}