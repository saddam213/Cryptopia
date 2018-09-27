using Cryptopia.Common.Balance;
using System;
using System.Linq;
using System.Threading.Tasks;
using Cryptopia.Common.DataContext;
using Cryptopia.Infrastructure.Common.DataTables;
using Cryptopia.Infrastructure.Common.DataContext;
using Cryptopia.Common.User;
using Cryptopia.Base;

namespace Cryptopia.Core.Balance
{
	public class BalanceReader : IBalanceReader
	{
		public IBalanceEstimationService BalanceEstimationService { get; set; }
		public IExchangeDataContextFactory ExchangeDataContextFactory { get; set; }

		public async Task<DataTablesResponse> GetTradeBalances(string userId, DataTablesModel model)
		{
			var currentUser = new Guid(userId);
			using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
			{
				var query = from currency in context.Currency
					from balance in context.Balance.Where(b => b.UserId == currentUser && b.CurrencyId == currency.Id).DefaultIfEmpty()
					where currency.IsEnabled
					select new BalanceTradeModel
					{
						CurrencyId = currency.Id,
						Symbol = currency.Symbol,
						Total = (decimal?) balance.Total ?? 0,
						HeldForTrades = (decimal?) balance.HeldForTrades ?? 0,
						PendingWithdraw = (decimal?) balance.PendingWithdraw ?? 0,
						Unconfirmed = (decimal?) balance.Unconfirmed ?? 0,
						Favorite = (bool?) balance.IsFavorite ?? false
					};

				var balances = await query.ToListNoLockAsync();
				await balances.ForEachAsync(async b => b.EstimatedBTC = await BalanceEstimationService.GetEstimatedBTC(b.Total, b.CurrencyId));
				return balances.GetDataTableResult(model, true);
			}
		}

		public async Task<BalanceCurrencyModel> GetCurrencyBalance(string userId, int currencyId)
		{
			var currentUser = new Guid(userId);
			using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
			{
				var query = from currency in context.Currency
					from balance in context.Balance.Where(b => b.UserId == currentUser && b.CurrencyId == currency.Id).DefaultIfEmpty()
					where currency.Id == currencyId
					select new BalanceCurrencyModel
					{
						CurrencyId = currency.Id,
						Symbol = currency.Symbol,
						Total = (decimal?) balance.Total ?? 0,
						HeldForTrades = (decimal?) balance.HeldForTrades ?? 0,
						PendingWithdraw = (decimal?) balance.PendingWithdraw ?? 0,
						Unconfirmed = (decimal?) balance.Unconfirmed ?? 0,
						MinTipAmount = currency.MinTip
					};
				return await query.FirstOrDefaultNoLockAsync().ConfigureAwait(false);
			}
		}

		public async Task<BalanceTradePairModel> GetTradePairBalance(string userId, int tradePairId)
		{
			var currentUser = new Guid(userId);
			using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
			{
				var query = from tradePair in context.TradePair
					from balance1 in
						context.Balance.Where(b => b.UserId == currentUser && b.CurrencyId == tradePair.CurrencyId1).DefaultIfEmpty()
					from balance2 in
						context.Balance.Where(b => b.UserId == currentUser && b.CurrencyId == tradePair.CurrencyId2).DefaultIfEmpty()
					where tradePair.Id == tradePairId
					select new
					{
						Symbol = tradePair.Currency1.Symbol,
						Total = (decimal?) balance1.Total ?? 0,
						HeldForTrades = (decimal?) balance1.HeldForTrades ?? 0,
						PendingWithdraw = (decimal?) balance1.PendingWithdraw ?? 0,
						Unconfirmed = (decimal?) balance1.Unconfirmed ?? 0,
						BaseSymbol = tradePair.Currency2.Symbol,
						BaseTotal = (decimal?) balance2.Total ?? 0,
						BaseHeldForTrades = (decimal?) balance2.HeldForTrades ?? 0,
						BasePendingWithdraw = (decimal?) balance2.PendingWithdraw ?? 0,
						BaseUnconfirmed = (decimal?) balance2.Unconfirmed ?? 0,
					};
				var balanceData = await query.FirstOrDefaultNoLockAsync().ConfigureAwait(false);
				if (balanceData == null)
					return new BalanceTradePairModel();

				return new BalanceTradePairModel
				{
					TradePairId = tradePairId,
					Symbol = balanceData.Symbol,
					BaseSymbol = balanceData.BaseSymbol,
					Available = balanceData.Total - (balanceData.PendingWithdraw + balanceData.HeldForTrades + balanceData.Unconfirmed),
					BaseAvailable =	balanceData.BaseTotal - (balanceData.BasePendingWithdraw + balanceData.BaseHeldForTrades + balanceData.BaseUnconfirmed),
					HeldForOrders = balanceData.HeldForTrades,
					BaseHeldForOrders = balanceData.BaseHeldForTrades
				};
			}
		}
	}
}