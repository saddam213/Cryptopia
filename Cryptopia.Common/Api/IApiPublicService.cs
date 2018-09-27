using System.Threading.Tasks;

namespace Cryptopia.Common.Api
{
	public interface IApiPublicService
	{
		Task<ApiCurrencyResult> GetCurrencies();
		Task<ApiTradePairResult> GetTradePairs();
		Task<ApiMarketsResult> GetMarkets(string basemarket, int hours);
		Task<ApiMarketResult> GetMarket(string market, int hours);

		Task<ApiMarketHistoryResult> GetMarketHistory(string market, int hours);

		Task<ApiMarketOrdersResult> GetMarketOrders(string market, int orders);
		Task<ApiMarketOrderGroupResult> GetMarketOrderGroup(string markets, int orders);
	}
}
