using Cryptopia.Infrastructure.Common.DataContext;
﻿using Cryptopia.Common.Currency;
using Cryptopia.Common.DataContext;
using Cryptopia.Common.Deposit;
using Cryptopia.Common.Trade;
using Cryptopia.Common.Withdraw;
using Cryptopia.Enums;
using System;
using System.Data.Entity;
using System.Threading.Tasks;
using Cryptopia.Infrastructure.Common.Results;

namespace Cryptopia.Core.Withdraw
{
	public class WithdrawWriter : IWithdrawWriter
	{
		public ICurrencyReader CurrencyReader { get; set; }
		public ITradeService TradeService { get; set; }
		public IDepositService DepositService { get; set; }
		public IExchangeDataContextFactory ExchangeDataContextFactory { get; set; }

		public async Task<IWriterResult<int>> CreateWithdraw(string userId, CreateWithdrawModel model)
		{
			var currentUser = new Guid(userId);
			var currency = await CurrencyReader.GetCurrency(model.CurrencyId);
			if(currency == null)
				return new WriterResult<int>(false, $"Currency not available for withdraw, Status: Not Found");

			using (var context = ExchangeDataContextFactory.CreateContext())
			{
				var user = await context.Users.FirstOrDefaultAsync(x => x.Id == currentUser).ConfigureAwait(false);
				if (user == null || string.IsNullOrEmpty(model.TwoFactorToken))
					return new WriterResult<int>(false, "Unauthorized.");

				var balance = await context.Balance.FirstOrDefaultAsync(x => x.UserId == user.Id && x.CurrencyId == model.CurrencyId).ConfigureAwait(false);
				if (balance == null || model.Amount > balance.Available)
					return new WriterResult<int>(false, "Insufficient funds.");

				if (currency.Status == CurrencyStatus.Maintenance || currency.Status == CurrencyStatus.Offline || currency.Status == CurrencyStatus.NoConnections)
					return new WriterResult<int>(false, $"Currency not available for withdraw, Status: {currency.Status}");

				if (model.Amount < currency.WithdrawMin || model.Amount >= currency.WithdrawMax)
					return new WriterResult<int>(false, "Withdrawal amount must be between {0} and {1} {2}", currency.WithdrawMin, currency.WithdrawMax, currency.Symbol);

				// Check address is in whitelist if whitelist enabled
				var safeAddressExists =
					await
						context.AddressBook.AnyNoLockAsync(
							x => x.UserId == user.Id && x.CurrencyId == balance.CurrencyId && x.Address == model.Address && x.IsEnabled)
							.ConfigureAwait(false);
				if (!user.IsUnsafeWithdrawEnabled && !safeAddressExists)
				{
					return new WriterResult<int>(false, "Address does not exist in your secure Withdraw Address Book.");
				}

				// Validate the address
				if (!await DepositService.ValidateAddress(balance.CurrencyId, model.Address).ConfigureAwait(false))
					return new WriterResult<int>(false, "Invalid {0} address.", currency.Symbol);

				var response = await TradeService.CreateWithdraw(userId, new CreateWithdrawModel
				{
					Address = model.Address,
					Amount = Math.Max(0, model.Amount),
					CurrencyId = balance.CurrencyId,
					TwoFactorToken = model.TwoFactorToken,
					Type = model.Type
				}).ConfigureAwait(false);

				if (response.IsError)
					return new WriterResult<int>(false, response.Error);

				return new WriterResult<int>(true, response.WithdrawId);
			}
		}

		public async Task<IWriterResult> ConfirmWithdraw(string userId, ConfirmWithdrawModel model)
		{
			var currentUser = new Guid(userId);
			using (var context = ExchangeDataContextFactory.CreateContext())
			{
				var user = await context.Users.FirstOrDefaultAsync(x => x.Id == currentUser).ConfigureAwait(false);
				if (user == null || string.IsNullOrEmpty(model.TwoFactorToken))
					return new WriterResult<int>(false, "Unauthorized");

				var withdraw =
					await
						context.Withdraw.FirstOrDefaultAsync(
							w => w.UserId == user.Id && w.Id == model.WithdrawId && w.Status == WithdrawStatus.Unconfirmed)
							.ConfigureAwait(false);
				if (withdraw == null)
					return new WriterResult(false, $"Withdraw #{model.WithdrawId} not found or is already confirmed.");

				if (!model.TwoFactorToken.Equals(withdraw.TwoFactorToken))
					return new WriterResult(false,
						$"Invalid or expired two factor token, you can send a new token from your WithdawHistory page.");

				withdraw.Confirmed = DateTime.UtcNow;
				withdraw.Status = WithdrawStatus.Pending;
				await context.SaveChangesAsync().ConfigureAwait(false);
				return new WriterResult(true);
			}
		}

		public async Task<IWriterResult> CancelWithdraw(string userId, CancelWithdrawModel model)
		{
			var currentUser = new Guid(userId);
			using (var context = ExchangeDataContextFactory.CreateContext())
			{
				var user = await context.Users.FirstOrDefaultAsync(x => x.Id == currentUser).ConfigureAwait(false);
				if (user == null)
					return new WriterResult<int>(false, "Unauthorized");

				var withdraw =
					await
						context.Withdraw.FirstOrDefaultAsync(
							w =>
								w.UserId == user.Id && w.Id == model.WithdrawId && w.Status == WithdrawStatus.Unconfirmed &&
								string.IsNullOrEmpty(w.Txid)).ConfigureAwait(false);
				if (withdraw == null)
					return new WriterResult(false, $"Withdraw #{model.WithdrawId} not found or is already canceled or processed.");

				withdraw.Status = WithdrawStatus.Canceled;
				await context.SaveChangesAsync().ConfigureAwait(false);
				await context.AuditUserBalance(user.Id, withdraw.CurrencyId).ConfigureAwait(false);
				return new WriterResult(true);
			}
		}

		public async Task<IWriterResult> UpdateTwoFactorToken(string userId, UpdateTwoFactorTokenModel model)
		{
			var currentUser = new Guid(userId);
			using (var context = ExchangeDataContextFactory.CreateContext())
			{
				var user = await context.Users.FirstOrDefaultAsync(x => x.Id == currentUser).ConfigureAwait(false);
				if (user == null)
					return new WriterResult<int>(false, "Unauthorized");

				var withdraw = await context.Withdraw
					.Include(x => x.Currency.Settings)
					.FirstOrDefaultAsync(w => w.UserId == user.Id && w.Id == model.WithdrawId && w.Status == WithdrawStatus.Unconfirmed)
					.ConfigureAwait(false);
				if (withdraw == null)
					return new WriterResult(false, $"Withdraw #{model.WithdrawId} not found or is already confirmed.");

				withdraw.TwoFactorToken = model.TwoFactorToken;
				await context.SaveChangesAsync().ConfigureAwait(false);

				model.Amount = withdraw.Amount - withdraw.Fee;
				model.Address = withdraw.Address;
				model.Symbol = withdraw.Currency.Symbol;
				model.AddressType = withdraw.Currency.Settings.AddressType;
				return new WriterResult(true);
			}
		}
	}
}
