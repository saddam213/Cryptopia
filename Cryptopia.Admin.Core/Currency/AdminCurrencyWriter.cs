using Cryptopia.Admin.Common.AdminCurrency;
using Cryptopia.Common.Cache;
using Cryptopia.Common.Currency;
using Cryptopia.Common.DataContext;
using Cryptopia.Infrastructure.Common.DataContext;
using Cryptopia.Infrastructure.Common.Results;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Cryptopia.Enums;
using Cryptopia.Infrastructure.Common.Email;
using Cryptopia.Infrastructure.Email;

namespace Cryptopia.Admin.Core.Currency
{
	public class AdminCurrencyWriter : IAdminCurrencyWriter
	{
		public ICacheService CacheService { get; set; }
        public IDataContextFactory DataContextFactory { get; set; }
		public IExchangeDataContextFactory ExchangeDataContextFactory { get; set; }
		public IEmailService EmailService { get; set; }

		public async Task<IWriterResult> UpdateCurrency(string adminUserId, UpdateCurrencyModel model)
		{
			try
			{
				using (var context = ExchangeDataContextFactory.CreateContext())
				{
					var currency =
						await context.Currency.Where(c => c.Id == model.Id).FirstOrDefaultNoLockAsync().ConfigureAwait(false);
					if (currency == null)
						return new WriterResult(false, "Currency not found");

					currency.PoolFee = model.PoolFee;
					currency.TradeFee = model.TradeFee;
					currency.WithdrawFee = model.WithdrawFee;
					currency.WithdrawFeeType = model.WithdrawFeeType;
					currency.MinWithdraw = model.WithdrawMin;
					currency.MaxWithdraw = model.WithdrawMax;
					currency.MinTip = model.TipMin;
					currency.MinBaseTrade = model.MinBaseTrade;
					currency.MinConfirmations = model.MinConfirmations;
					currency.Status = model.Status;
					currency.StatusMessage = model.StatusMessage;
					currency.ListingStatus = model.ListingStatus;

                    using (var adminContext = DataContextFactory.CreateContext())
                    {
                        adminContext.LogActivity(adminUserId, $"Updated Currency: {currency.Symbol}");
                    }

                    await context.SaveChangesAsync().ConfigureAwait(false);
					await CacheService.InvalidateAsync(CacheKey.Currencies(), CacheKey.CurrencyInfo(), CacheKey.CurrencyDataTable(), CacheKey.CurrencySummary(model.Id)).ConfigureAwait(false);
					return new WriterResult(true, "Succesfully updated currency settings.");
				}
			}
			catch (Exception)
			{
				return null;
			}
		}

		private async Task<IWriterResult> UpdateListingStatus(string adminUserId, UpdateListingStatusModel model)
		{
			try
			{
				using (var context = ExchangeDataContextFactory.CreateContext())
				{
					var currency =
						await context.Currency.Where(c => c.Id == model.CurrencyId).FirstOrDefaultNoLockAsync().ConfigureAwait(false);
					if (currency == null)
						return new WriterResult(false, "Currency not found");

                    var oldStatus = currency.ListingStatus;
					currency.StatusMessage = model.StatusMessage;
					currency.ListingStatus = model.ListingStatus;
					currency.Settings.DelistOn = model.DelistOn;

                    using (var adminContext = DataContextFactory.CreateContext())
                    {
                        adminContext.LogActivity(adminUserId, $"Updated Currency listing status from : {oldStatus} to: {model.ListingStatus}");
                        await adminContext.SaveChangesAsync().ConfigureAwait(false);
                    }

                    await context.SaveChangesAsync().ConfigureAwait(false);
					await CacheService.InvalidateAsync(CacheKey.Currencies(), CacheKey.CurrencyInfo(), CacheKey.CurrencyDataTable(), CacheKey.CurrencySummary(model.CurrencyId)).ConfigureAwait(false);
					return new WriterResult(true, "Succesfully updated listing status.");
				}
			}
			catch (Exception)
			{
				return null;
			}
		}

		public async Task<IWriterResult> BeginDelistingCurrency(string adminUserId, UpdateListingStatusModel model)
		{
			model.ListingStatus = CurrencyListingStatus.Delisting;

			if (model.DelistOn == null) return new WriterResult(false, "Delist date cannot be null");

			var writerResult = await UpdateListingStatus(adminUserId, model);

			if (!writerResult.Success) return writerResult;

			using (var context = ExchangeDataContextFactory.CreateContext())
			{
				// Checks for open trade pairs only to avoid accidentally relisting delisted currencies.
				var tradePairs = await context.TradePair.Where(t => (t.Status == TradePairStatus.OK || t.Status == TradePairStatus.Paused) && (t.CurrencyId1 == model.CurrencyId || t.CurrencyId2 == model.CurrencyId)).ToListNoLockAsync();
				foreach (var tradePair in tradePairs)
				{
					tradePair.Status = TradePairStatus.Closing;
				}

                using (var adminContext = DataContextFactory.CreateContext())
                {
                    adminContext.LogActivity(adminUserId, "Closing TradePair due to delisting");
                    await adminContext.SaveChangesAsync().ConfigureAwait(false);
                }

                await context.SaveChangesAsync().ConfigureAwait(false);
				await CacheService.InvalidateAsync(CacheKey.AllTradePairs(), CacheKey.TradePairs()).ConfigureAwait(false);
			}

			writerResult.Message = "Successfully started the delisting process.";

			try
			{
				var email = new EmailMessageModel
				{
					EmailType = EmailTemplateType.CoinDelisting,
					BodyParameters = new object[0],
					SystemIdentifier = model.Symbol
				};

				using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
				{
					var balances = await context.Balance.Include(b => b.User).Where(b => b.Currency.Name == model.Name && b.Total > 0).OrderBy(b => b.User.Email).ToListNoLockAsync();
					if (!balances.Any())
					{
						writerResult.Message = "Successfully started the delisting process but no non zero balances found so no emails were sent.";
						return writerResult;
					}

					var personalisations = new List<IEmailPersonalisation>(balances.Select(balance => new EmailPersonalisation{
							Tos = new List<string> { balance.User.Email},
							Substitutions = new Dictionary<string, string>
							{
								{"-name-", balance.User.UserName},
								{"-coinName-", model.Name},
								{"-coinSymbol-", model.Symbol},
								{"-delistDate-", model.DelistOn.Value.ToString("R")},
								{"-delistReason-", model.StatusMessage},
								{"-balance-", balance.Total.ToString(CultureInfo.InvariantCulture)}
							}
						})
						.ToList());

					var hasSentSuccessfully = await EmailService.SendEmails(email, personalisations);

					if (!hasSentSuccessfully)
					{
						writerResult.Success = false;
						writerResult.Message = "Successfully started the delisting process but emails failed to send.";
						return writerResult;
					}
				}

                using (var adminContext = DataContextFactory.CreateContext())
                {
                    adminContext.LogActivity(adminUserId, $"Started Currency Delisting Process for: {model.Name}");
                    await adminContext.SaveChangesAsync().ConfigureAwait(false);
                }
            }
			catch (Exception)
			{
				return new WriterResult(false, "Sending emails failed");
			}
			return writerResult;
		}

		public async Task<IWriterResult> DelistCurrency(string adminUserId, UpdateListingStatusModel model)
		{
			model.ListingStatus = CurrencyListingStatus.Delisted;
			var writerResult = await UpdateListingStatus(adminUserId, model);
			if (!writerResult.Success) return writerResult;

			using (var context = ExchangeDataContextFactory.CreateContext())
			{
				// Checks for closing trade pairs only as it's expected to be in 'delisting' before it's delisted.
				var tradePairs = await context.TradePair.Where(t => t.Status == TradePairStatus.Closing && (t.CurrencyId1 == model.CurrencyId || t.CurrencyId2 == model.CurrencyId)).ToListNoLockAsync();
				foreach (var tradePair in tradePairs)
				{
					tradePair.Status = TradePairStatus.Closed;
				}

                using (var adminContext = DataContextFactory.CreateContext())
                {
                    adminContext.LogActivity(adminUserId, $"Delisted Currency: {model.Name}");
                    await adminContext.SaveChangesAsync().ConfigureAwait(false);
                }

                await context.SaveChangesAsync().ConfigureAwait(false);
				await CacheService.InvalidateAsync(CacheKey.Currencies(), CacheKey.CurrencyInfo(), CacheKey.CurrencyDataTable(), CacheKey.CurrencySummary(model.CurrencyId)).ConfigureAwait(false);
			}

			writerResult.Message = "Successfully delisted currency.";

			return writerResult;
		}
	}
}
