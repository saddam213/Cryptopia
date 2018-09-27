using System;
using Cryptopia.Enums;

namespace Cryptopia.Common.Cache
{
	public static class CacheKey
	{
		public static string TradePairs()
		{
			return "tradepairs";
		}

		public static string TradePairLastPrice(int tradePairid)
		{
			return $"tradepairlastprice-{tradePairid}";
		}

		public static string CurrencyDataTable()
		{
			return "CurrencyDataTable";
		}

		public static string Currencies()
		{
			return "currencies";
		}

		public static string ArbitrageData()
		{
			return "arbitragedata";
		}

		public static string Emoticons()
		{
			return "emoticons";
		}

		public static string MarketCategories()
		{
			return "marketcategories";
		}

		public static string BaseCurrencies()
		{
			return "basecurrencies";
		}



		public static string Locations()
		{
			return "locations";
		}

		public static string CurrencySummary(int currencyId)
		{
			return $"currencysummary-{currencyId}";
		}

		public static string ExchangeTradeHistoryData()
		{
			return $"exchangetradehistorydata";
		}

		public static string CurrencyInfo()
		{
			return "currencyinfo";
		}

		public static string ExchangeSummary()
		{
			return "currencysummary";
		}

		public static string ConvertDollarToBTC(decimal dollarAmount, ConvertDollarToBTCType dollarType)
		{
			return $"convertdollartobtc-{dollarType}-{dollarAmount}";
		}
		
		public static string ExchangeSummary(string baseCurrency)
		{
			return $"currencysummary-{baseCurrency}";
		}



		public static string ExchangeStockChart(int tradePairId, int dataRange, int dataGroups)
		{
			return $"exchange-stockchart-{tradePairId}-{dataRange}-{dataGroups}";
		}

		public static string ExchangeDepthChart(int tradePairId)
		{
			return $"exchange-depthchart-{tradePairId}";
		}


		public static string ExchangeDistributionChart(int currencyId, ChartDistributionCount count)
		{
			return $"exchange-distributionchart-{currencyId}-{count}";
		}

		public static string ExchangeTradeHistory(int tradePairId)
		{
			return $"exchange-tradehistory-{tradePairId}";
		}




		public static string ExchangeUserOpenOrderDataTable(string userId)
		{
			return $"exchange-useropenorder-datatabe-{userId}";
		}


		public static string ChatEmoticons()
		{
			return "chatemoticons";
		}

		public static string ExchangeUserTradeHistory(string userId, int tradePairId)
		{
			return $"exchange-usertradehistory-{userId}-{tradePairId}";
		}


		public static string ExchangeUserOpenOrders(string userId, int tradePairId)
		{
			return $"exchange-useropenorders-{userId}-{tradePairId}";
		}

		public static string ExchangeOrderBook(int tradePairId)
		{
			return $"exchange-orderbook-{tradePairId}";
		}

		public static string ExchangeUserOrderBook(string userId, int tradePairId)
		{
			return $"exchange-userorderbook-{userId}-{tradePairId}";
		}


		//public static string ApiUserBalance(string userId)
		//{
		//	return $"api-userbalance-{userId}";
		//}

		//public static string ApiUserBalance(string userId, int currencyId)
		//{
		//	return $"api-userbalance-{userId}-{currencyId}";
		//}

		public static string ApiMarketOrdersByTradePair(int tradePairId, int orderCount)
		{
			return $"api-marketorders-{tradePairId}-{orderCount}";
		}

		public static string ApiMarket(int hours, int tradePairId)
		{
			return $"api-maeket-{hours}-{tradePairId}";
		}



		public static string ApiMarketHistory(int tradePairId, int hours)
		{
			return $"api-markethistory-{tradePairId}-{hours}";
		}

	
		public static string ApiMarkets(string baseMarket, int hours)
		{
			return $"api-markets-{baseMarket}-{hours}";
		}

		public static string AllTradePairs()
		{
			return "alltradepairs";
		}

		public static string NZDPerCoin(int currencyId)
		{
			return $"nzdpercoin-{currencyId}";
		}

		//public static string ApiUserOpenOrders(string userId, int tradePairId)
		//{
		//	return $"api-useropenorders-{userId}-{tradePairId}";
		//}

		//public static string ApiUserOpenOrders(string userId)
		//{
		//	return $"api-useropenorders-{userId}";
		//}

		public static string ApiUserTradeHistory(string userId, int tradePairId)
		{
			return $"api-usertradehistory-{userId}-{tradePairId}";
		}

		public static string ApiUserTradeHistory(string userId)
		{
			return $"api-usertradehistory-{userId}";
		}

		public static string ApiUserTransactions(string userId, TransactionType type)
		{
			return $"api-usertransactions-{userId}-{type}";
		}

		public static string CurrencyPeerInfo(int currencyId)
		{
			return $"currencypeerinfo-{currencyId}";
		}

		public static string MineshaftBlockChart(int poolId)
		{
			return $"mineshaftblockchart-{poolId}";
		}



		public static string MineshaftMiners(int poolId)
		{
			return $"mineshaftminers-{poolId}";
		}

		public static string MineshaftUserInfo(int poolId, string userId)
		{
			return $"mineshaftuserinfo-{poolId}-{userId}";
		}

		public static string MineshaftSummary()
		{
			return $"mineshaftsummary";
		}

		public static string MineshaftSummary(AlgoType? algoType)
		{
			var key = algoType.HasValue ? algoType.Value.ToString() : string.Empty;
			return $"mineshaftsummary-{key}";
		}

		public static string MineshaftInfo(int poolId)
		{
			return $"mineshaftinfo-{poolId}";
		}

		public static string Pools()
		{
			return $"pools";
		}

		public static string Pool(int poolId)
		{
			return $"pool-{poolId}";
		}
		public static string PoolConnections()
		{
			return $"poolconnections";
		}
		public static string PoolConnection(AlgoType algoType)
		{
			return $"poolconnection-{algoType}";
		}

		public static string PoolWorkers(string userId)
		{
			return $"poolworkers-{userId}";
		}

		public static string PoolWorkers(int poolId)
		{
			return $"poolworkers-{poolId}";
		}

		public static string MineshaftHashrateChart(int poolId, Guid userId)
		{
			return $"mineshafthashratechart-{poolId}-{userId}";
		}

		public static string ApiMarketOrderGroup(string tradePairs, int orderCount)
		{
			return $"apimarkerordergroup-{tradePairs}-{orderCount}";
		}

		public static string PaytopiaItems()
		{
			return "paytopiaitems";
		}
		public static string PaytopiaItem(PaytopiaItemType itemType)
		{
			return $"paytopiaitem-{itemType}";
		}

		public static string TradePairTicker(int tradePairId)
		{
			return $"tradepairticker-{tradePairId}";
		}

		public static string ShareholderReferralExpenses()
		{
			return $"shareholderreferralexpenses";
		}

		public static string ShareholderSiteExpenses()
		{
			return $"shareholdersiteexpenses";
		}

		public static string ShareholderTradeFeeInfo()
		{
			return $"shareholdertradefeeinfo";
		}
		public static string ShareholderPaytopiaFeeInfo()
		{
			return $"shareholderpaytopiafeeinfo";
		}

        public static string WalletTransactions(string currencySymbol)
        {
            return $"wallet-tx-{currencySymbol}";
        }

        public static string AdminActivityReports()
        {
            return $"adminActivityReports";
        }
	}
}
