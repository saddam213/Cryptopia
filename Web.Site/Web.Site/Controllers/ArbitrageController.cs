using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Web.Site.Models;
using Cryptopia.Common.Currency;
using Cryptopia.Common.Arbitrage;

namespace Web.Site.Controllers
{
	public class ArbitrageController : BaseController
	{
		public IArbitrageReader ArbitrageReader { get; set; }
		public ICurrencyReader CurrencyReader { get; set; }

		[HttpGet]
		[AllowAnonymous]
		public async Task<ActionResult> Index()
		{
			var data = await ArbitrageReader.GetData();
			var baseCurrencies = await CurrencyReader.GetBaseCurrencies();
			var arbitrageBases = data.Select(x => x.BaseCurrencyId).Distinct();
			return View("Arbitrage", new ArbitrageModel
			{
				Exchanges = data.Select(x => x.Exchange).Distinct().ToList(),
				BaseCurrencies = baseCurrencies.Where(x => arbitrageBases.Contains(x.CurrencyId)).ToList()
			});
		}

		[HttpGet]
		[AllowAnonymous]
		public async Task<ActionResult> GetArbitrageSummary(int currencyId)
		{
			var data = await ArbitrageReader.GetData();
			var arbCurrencies = data.Where(x => x.BaseCurrencyId == currencyId).Select(x => x.CurrencyId).Distinct().ToList();
			var currencies = (await CurrencyReader.GetCurrencies()).Where(x => arbCurrencies.Contains(x.CurrencyId));
			var exchanges = data.Select(x => x.Exchange).GroupBy(x => x).OrderByDescending(x => x.Count()).Select(x => x.Key).ToList();
			var marketData = new Dictionary<CurrencyModel, List<ArbitrageDataModel>>();
			foreach (var currency in currencies)
			{
				var currencyData = new List<ArbitrageDataModel>();
				foreach (var exchange in exchanges)
				{
					currencyData.Add(data.FirstOrDefault(x => x.Symbol == currency.Symbol && x.BaseCurrencyId == currencyId && x.Exchange == exchange) ?? new ArbitrageDataModel{ Exchange = exchange});
				}
				marketData.Add(currency, currencyData);
			}

			return PartialView("_ArbitrageMarket", new ArbitrageMarketModel
			{
				Exchanges = exchanges,
				Markets = marketData
			});
		}
	}
}