using System;
using System.Linq;
using System.Threading.Tasks;
using Cryptopia.Common.User;
using Cryptopia.Infrastructure.Common.DataContext;
using Cryptopia.Common.Trade;
using Cryptopia.Enums;
using System.Data.Entity;
using Cryptopia.Common.DataContext;
using Cryptopia.Infrastructure.Common.Results;

namespace Cryptopia.Core.User
{
	public class UserBalanceWriter : IUserBalanceWriter
	{

		public ITradeService TradeService { get; set; }
		public IExchangeDataContextFactory ExchangeDataContextFactory { get; set; }

		public async Task<IWriterResult> DustBalance(string userId, int currencyId)
		{
			try
			{
				var symbol = "";
				var available = 0m;
				var currentUser = new Guid(userId);
				using (var context = ExchangeDataContextFactory.CreateContext())
				{
					var balance = await context.Balance
						.Include(x => x.Currency)
						.Where(b => b.UserId == currentUser && b.CurrencyId == currencyId)
						.FirstOrDefaultNoLockAsync().ConfigureAwait(false);
					if (balance == null || balance.Total <= 0)
						return new WriterResult(false, "No funds found to dust.");

					if(balance.HeldForTrades > 0)
						return new WriterResult(false, "You have funds held for trades, please cancel any open orders before dusting balance.");

					if (balance.PendingWithdraw > 0)
						return new WriterResult(false, "You have funds pending withdrawal, please cancel/confirm any pending withdrawals before dusting balance.");

					if (balance.Unconfirmed > 0)
						return new WriterResult(false, "You have unconfirmed deposits, please wait for full confirmations on the pending deposits before dusting balance.");

					symbol = balance.Currency.Symbol;
					available = balance.Available;
				}

				var result = await TradeService.CreateTransfer(userId, new CreateTransferModel
				{
					Amount = available,
					CurrencyId = currencyId,
					Receiver = Constant.SYSTEM_USER_DUSTBIN.ToString(),
					TransferType = TransferType.DustBin
				}).ConfigureAwait(false);

				if (result.IsError )
					return new WriterResult(false, result.Error);

				return new WriterResult(true, $"Successfully sent {available:F8} {symbol} to the dustbin.");
			}
			catch (Exception)
			{
				return new WriterResult(false);
			}
		}

		public async Task<IWriterResult> SetFavorite(string userId, int currencyId)
		{
			try
			{
				var currentUser = new Guid(userId);
				using (var context = ExchangeDataContextFactory.CreateContext())
				{
					var balance = await context.Balance
							.Where(b => b.UserId == currentUser && b.CurrencyId == currencyId)
							.FirstOrDefaultNoLockAsync().ConfigureAwait(false);
					if (balance == null)
					{
						balance = new Entity.Balance
						{
							UserId = currentUser,
							CurrencyId = currencyId
						};
						context.Balance.Add(balance);
					}

					balance.IsFavorite = !balance.IsFavorite;
					await context.SaveChangesAsync().ConfigureAwait(false);
					return new WriterResult(true);
				}
			}
			catch (Exception)
			{
				return new WriterResult(false);
			}
		}
	}
}
