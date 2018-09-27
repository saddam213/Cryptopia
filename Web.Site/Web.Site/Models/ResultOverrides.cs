using Cryptopia.Common.User;
using Cryptopia.Common.Currency;
using Cryptopia.Common.Arbitrage;

namespace Web.Site
{
	public static class ResultExtesions
	{
		public static string ToCurrencyLabel(this CurrencyModel currency)
		{
			if (currency != null)
			{
				return string.Format("{0}({1})", currency.Name, currency.Symbol);
			}
			return string.Empty;
		}



		public static string ToCurrencyLabel(this UserBalanceItemModel currency)
		{
			if (currency != null)
			{
				return string.Format("{0}({1})", currency.Name, currency.Symbol);
			}
			return string.Empty;
		}



		public static bool IsZeroBalance(this UserBalanceItemModel balance)
		{
			return (balance.PendingWithdraw <= 0 && balance.PoolPending <= 0 && balance.HeldForTrades <= 0 && balance.Available <= 0);
		}

		

		public static bool HasData(this ArbitrageDataModel result)
		{
			if (result != null)
			{
				return !string.IsNullOrEmpty(result.MarketUrl);
			}
			return false;
		}
	}
}