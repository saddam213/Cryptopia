using Cryptopia.Common.Api;
using System;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using WebApi.OutputCache.V2;

namespace Web.Site.Api.Controllers
{
	[EnableCors(origins: "*", headers: "*", methods: "get")]
	public class ApiPublicController : ApiController
	{
		public IApiPublicService ApiPublicService { get; set; }

		[HttpGet]
	//	[CacheOutput(ClientTimeSpan = 120, ServerTimeSpan = 120, ExcludeQueryStringFromCacheKey = true)]
		public async Task<ApiTradePairResult> GetTradePairs()
		{
			try
			{
				return await ApiPublicService.GetTradePairs();
			}
			catch (Exception)
			{
				return new ApiTradePairResult { Success = false, Message = "Bad Request" };
			}
		}

		[HttpGet]
	//	[CacheOutput(ClientTimeSpan = 120, ServerTimeSpan = 120, ExcludeQueryStringFromCacheKey = true)]
		public async Task<ApiCurrencyResult> GetCurrencies()
		{
			try
			{
				return await ApiPublicService.GetCurrencies();
			}
			catch (Exception)
			{
				return new ApiCurrencyResult { Success = false, Message = "Bad Request" };
			}
		}

		[HttpGet]
	//	[CacheOutput(ClientTimeSpan = 20, ServerTimeSpan = 20)]
		public async Task<ApiMarketsResult> GetMarkets(string baseMarket = null, int? hours = null)
		{
			try
			{
				var marketHours = hours ?? 24;
				if (int.TryParse(baseMarket, out marketHours))
				{
					return await ApiPublicService.GetMarkets(string.Empty, marketHours);
				}
				return await ApiPublicService.GetMarkets(baseMarket, hours ?? 24);
			}
			catch (Exception)
			{
				return new ApiMarketsResult { Success = false, Message = "Bad Request" };
			}
		}

		[HttpGet]
	//	[CacheOutput(ClientTimeSpan = 20, ServerTimeSpan = 20)]
		public async Task<ApiMarketResult> GetMarket(string market, int? hours = null)
		{
			try
			{
				return await ApiPublicService.GetMarket(market, hours ?? 24);
			}
			catch (Exception)
			{
				return new ApiMarketResult { Success = false, Message = "Bad Request" };
			}
		}

		[HttpGet]
//		[CacheOutput(ClientTimeSpan = 20, ServerTimeSpan = 20)]
		public async Task<ApiMarketHistoryResult> GetMarketHistory(string market, int? hours = null)
		{
			try
			{
				return await ApiPublicService.GetMarketHistory(market, hours ?? 24);
			}
			catch (Exception)
			{
				return new ApiMarketHistoryResult { Success = false, Message = "Bad Request" };
			}
		}

		[HttpGet]
	//	[CacheOutput(ClientTimeSpan = 1, ServerTimeSpan = 1)]
		public async Task<ApiMarketOrdersResult> GetMarketOrders(string market, int? orderCount)
		{
			try
			{
				return await ApiPublicService.GetMarketOrders(market, orderCount ?? 100);
			}
			catch (Exception)
			{
				return new ApiMarketOrdersResult { Success = false, Message = "Bad Request" };
			}
		}

		[HttpGet]
	//	[CacheOutput(ClientTimeSpan = 1, ServerTimeSpan = 1)]
		public async Task<ApiMarketOrderGroupResult> GetMarketOrderGroups(string markets, int? orderCount)
		{
			try
			{
				return await ApiPublicService.GetMarketOrderGroup(markets, orderCount ?? 100);
			}
			catch (Exception)
			{
				return new ApiMarketOrderGroupResult { Success = false, Message = "Bad Request" };
			}
		}
	}
}