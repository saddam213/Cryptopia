using System.Collections.Generic;
using System.Threading.Tasks;
using Cryptopia.Infrastructure.Common.DataTables;
using Cryptopia.Common.TradeNotification;

namespace Cryptopia.Common.TradePair
{
	public interface ITradePairReader
	{
		Task<TradePairModel> GetTradePair(string market, bool includeClosed = false);
		Task<TradePairModel> GetTradePair(int tradePairId, bool includeClosed = false);
		Task<TradePairModel> GetTradePair(int currency, int baseCurrency, bool includeClosed = false);
		Task<TradePairModel> GetTradePair(string currency, string baseCurrency, bool includeClosed = false);
		Task<List<TradePairModel>> GetTradePairs(bool includeClosed = false);
		Task<TradePairTickerModel> GetTicker(int tradePairId);
		//Task UpdatePriceCache(TradePriceUpdate priceUpdate);

		Task<UpdateTradePairModel> AdminGetTradePair(int id);
		Task<DataTablesResponse> AdminGetTradePairs(DataTablesModel model);
	}
}
