using System;
using System.Linq;
using System.Threading.Tasks;
using Cryptopia.Common.User;
using Cryptopia.Common.DataContext;
using Cryptopia.Common.Balance;
using Cryptopia.Enums;
using Cryptopia.Base;
using Cryptopia.Common.UserVerification;
using Cryptopia.Infrastructure.Common.DataContext;

namespace Cryptopia.Core.User
{
	public class UserBalanceReader : IUserBalanceReader
	{
		public IDataContextFactory DataContextFactory { get; set; }
		public IExchangeDataContextFactory ExchangeDataContextFactory { get; set; }
		public IBalanceEstimationService BalanceEstimationService { get; set; }
		public IUserVerificationReader UserVerificationReader { get; set; }


		public async Task<UserBalanceItemModel> GetBalance(string userId, int currencyId)
		{
			try
			{
				var currentUser = new Guid(userId);
				using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
				{
					var balance = await context.Balance
						.AsNoTracking()
						.Where(x => x.UserId == currentUser && x.CurrencyId == currencyId)
						.Select(x => new UserBalanceItemModel
						{
							CurrencyId = x.CurrencyId,
							HeldForTrades = (decimal?)x.HeldForTrades ?? 0,
							PendingWithdraw = (decimal?)x.PendingWithdraw ?? 0,
							Name = x.Currency.Name,
							Symbol = x.Currency.Symbol,
							Status = x.Currency.Status,
							ListingStatus = x.Currency.ListingStatus,
							StatusMessage = x.Currency.StatusMessage,
							Total = (decimal?)x.Total ?? 0,
							Unconfirmed = (decimal?)x.Unconfirmed ?? 0,
							IsFavorite = (bool?)x.IsFavorite ?? false,
							CurrencyType = x.Currency.Type,
							BaseAddress = x.Currency.BaseAddress
						}).FirstOrDefaultNoLockAsync().ConfigureAwait(false);
					return balance;
				}
			}
			catch (Exception)
			{
				return new UserBalanceItemModel();
			}
		}

		public async Task<UserBalanceModel> GetBalances(string userId, bool calculateEstimate)
		{
			try
			{
				var model = new UserBalanceModel();
				var currentUser = new Guid(userId);
				using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
				{
					var query = from currency in context.Currency
											from balance in context.Balance.Where(b => b.UserId == currentUser && b.CurrencyId == currency.Id).DefaultIfEmpty()
												//	from address in context.Address.Where(a => a.UserId == currentUser && a.CurrencyId == currency.Id).DefaultIfEmpty()
											where currency.IsEnabled
											orderby currency.Name
											select new UserBalanceItemModel
											{
												//Address = address.AddressHash,
												CurrencyId = currency.Id,
												HeldForTrades = (decimal?)balance.HeldForTrades ?? 0,
												PendingWithdraw = (decimal?)balance.PendingWithdraw ?? 0,
												Name = currency.Name,
												Symbol = currency.Symbol,
												Status = currency.Status,
												ListingStatus = currency.ListingStatus,
												StatusMessage = currency.StatusMessage,
												Total = (decimal?)balance.Total ?? 0,
												Unconfirmed = (decimal?)balance.Unconfirmed ?? 0,
												IsFavorite = (bool?)balance.IsFavorite ?? false,
												CurrencyType = currency.Type,
												BaseAddress = currency.BaseAddress
											};

					var balances = await query.ToListNoLockAsync().ConfigureAwait(false);
					model.Balances = balances.DistinctBy(x => x.CurrencyId).ToList();
				}

				if (calculateEstimate)
				{
					await model.Balances.ForEachAsync(async b => b.EstimatedBTC = await BalanceEstimationService.GetEstimatedBTC(b.Total, b.CurrencyId));
					model.BTCEstimate = model.Balances.Sum(x => x.EstimatedBTC);
					model.BTCEstimateAlt = model.Balances.Where(x => x.CurrencyId != Constant.BITCOIN_ID).Sum(x => x.EstimatedBTC);

					var verificationData = await UserVerificationReader.GetVerificationStatus(userId).ConfigureAwait(false);
					model.HasWithdrawLimit = verificationData.Limit > 0;
					model.WithdrawLimit = verificationData.Limit;
					model.WithdrawTotal = verificationData.Current;
				}
				return model;
			}
			catch (Exception)
			{
				return new UserBalanceModel();
			}
		}
	}
}