using Cryptopia.Infrastructure.Common.DataTables;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cryptopia.Common.TradeNotification;
using Cryptopia.Enums;

namespace Cryptopia.Common.Exchange
{
	public interface IExchangeReader
	{
		Task<ExchangeSummary> GetExchangeSummary();
		Task<DataTablesResponse> GetExchangeSummary(DataTablesModel param, string baseCurrency);

		Task<StockChartDataModel> GetStockChart(int tradePairId, int dataRange, int dataGroup);
		Task<DistributionChartDataModel> GetDistributionChart(int currencyId, ChartDistributionCount count);

		Task<OrderBookModel> GetOrderBook(string userId, int tradePairId);

		Task<List<string[]>> GetTradeHistory(int tradePairId);
		Task<List<string[]>> GetUserOrders(string userId, int tradePairId);
		Task<List<string[]>> GetUserHistory(string userId, int tradePairId);
		Task<DataTablesResponse> GetUserOrders(DataTablesModel param, string userId);
		//Task UpdatePriceCache(TradePriceUpdate priceUpdate);
	}
}
