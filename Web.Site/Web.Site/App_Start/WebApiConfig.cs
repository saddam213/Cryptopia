using System.Collections.Generic;
using System.Configuration;
using System.Net.Http.Headers;
using System.Web.Http;
using Web.Site.Api.Implementation;
using Web.Site.Cache;
using WebApiThrottle;


namespace Web.Api
{
	public static class WebApiConfig
	{
		private static long _apiRequestPerDay = long.Parse(ConfigurationManager.AppSettings["Trottle_ApiRequestPerDay"]);
		private static long _apiRequestPerMinute = long.Parse(ConfigurationManager.AppSettings["Trottle_ApiRequestPerMinute"]);
		private static bool _useRedisCache = bool.Parse(ConfigurationManager.AppSettings["Redis_ApiThrottleCache_Enabled"]);
		public static void Register(HttpConfiguration config)
		{
			// Enable cors
			config.EnableCors();

			// Web API configuration and services
			config.MessageHandlers.Add(new ApiThrottleHandler
			{
				Policy = new ThrottlePolicy(perMinute: _apiRequestPerMinute, perDay: _apiRequestPerDay)
				{
					IpThrottling = false,
					ClientThrottling = true,
					EndpointThrottling = true,
					ClientWhitelist = new List<string> { "Public" },
					EndpointWhitelist = new List<string> { "Api/SubmitTip" }
				},
				Repository = _useRedisCache
					? new RedisThrottleRepository()
					: (IThrottleRepository)new CacheRepository(),
				QuotaExceededMessage = "API calls quota exceeded! maximum {0} calls per {1}"
			});

			// Web API routes
			config.MapHttpAttributeRoutes();
			config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));

			// Public API
			config.Routes.MapHttpRoute(name: "GetCurrencies", routeTemplate: "api/GetCurrencies", defaults: new { controller = "ApiPublic", action = "GetCurrencies" });
			config.Routes.MapHttpRoute(name: "GetTradePairs", routeTemplate: "api/GetTradePairs", defaults: new { controller = "ApiPublic", action = "GetTradePairs" });
			config.Routes.MapHttpRoute(name: "GetMarkets", routeTemplate: "api/GetMarkets/{baseMarket}/{hours}", defaults: new { controller = "ApiPublic", action = "GetMarkets", baseMarket = RouteParameter.Optional, hours = RouteParameter.Optional });
			config.Routes.MapHttpRoute(name: "GetMarket", routeTemplate: "api/GetMarket/{market}/{hours}", defaults: new { controller = "ApiPublic", action = "GetMarket", hours = RouteParameter.Optional });
			config.Routes.MapHttpRoute(name: "GetMarketHistory", routeTemplate: "api/GetMarketHistory/{market}/{hours}", defaults: new { controller = "ApiPublic", action = "GetMarketHistory", hours = RouteParameter.Optional });
			config.Routes.MapHttpRoute(name: "GetMarketOrders", routeTemplate: "api/GetMarketOrders/{market}/{orderCount}", defaults: new { controller = "ApiPublic", action = "GetMarketOrders", orderCount = 100 });
			config.Routes.MapHttpRoute(name: "GetMarketOrderGroups", routeTemplate: "api/GetMarketOrderGroups/{markets}/{orderCount}", defaults: new { controller = "ApiPublic", action = "GetMarketOrderGroups", orderCount = 100 });

			// Private Api
			config.Routes.MapHttpRoute(name: "SubmitTrade", routeTemplate: "Api/SubmitTrade", defaults: new { controller = "ApiPrivate", action = "SubmitTrade" });
			config.Routes.MapHttpRoute(name: "SubmitTransfer", routeTemplate: "Api/SubmitTransfer", defaults: new { controller = "ApiPrivate", action = "SubmitTransfer" });
			config.Routes.MapHttpRoute(name: "CancelTrade", routeTemplate: "Api/CancelTrade", defaults: new { controller = "ApiPrivate", action = "CancelTrade" });
			config.Routes.MapHttpRoute(name: "GetOpenOrders", routeTemplate: "Api/GetOpenOrders", defaults: new { controller = "ApiPrivate", action = "GetOpenOrders" });
			config.Routes.MapHttpRoute(name: "GetTradeHistory", routeTemplate: "Api/GetTradeHistory", defaults: new { controller = "ApiPrivate", action = "GetTradeHistory" });
			config.Routes.MapHttpRoute(name: "SubmitTip", routeTemplate: "Api/SubmitTip", defaults: new { controller = "ApiPrivate", action = "SubmitTip" });
			config.Routes.MapHttpRoute(name: "GetTransactions", routeTemplate: "Api/GetTransactions", defaults: new { controller = "ApiPrivate", action = "GetTransactions" });
			config.Routes.MapHttpRoute(name: "GetBalance", routeTemplate: "Api/GetBalance", defaults: new { controller = "ApiPrivate", action = "GetBalance" });
			config.Routes.MapHttpRoute(name: "GetDepositAddress", routeTemplate: "Api/GetDepositAddress", defaults: new { controller = "ApiPrivate", action = "GetDepositAddress" });
			config.Routes.MapHttpRoute(name: "SubmitWithdraw", routeTemplate: "Api/SubmitWithdraw", defaults: new { controller = "ApiPrivate", action = "SubmitWithdraw" });
		}
	}
}
