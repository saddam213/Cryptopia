using Cryptopia.Admin.Common.AdminCurrency;
using Cryptopia.Common.Cache;
using Cryptopia.Common.DataContext;
using Cryptopia.Common.TradePair;
using Cryptopia.Infrastructure.Common.DataContext;
using Cryptopia.Infrastructure.Common.DataTables;
using Cryptopia.Infrastructure.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace Cryptopia.Admin.Core.TradePair
{
	public class AdminTradePairWriter : IAdminTradePairWriter
	{
		public ICacheService CacheService { get; set; }
        public IDataContextFactory DataContextFactory { get; set; }
		public IExchangeDataContextFactory ExchangeDataContextFactory { get; set; }

		public async Task<IWriterResult> CreateTradePair(string adminUserId, CreateTradePairModel model)
		{
			using (var context = ExchangeDataContextFactory.CreateContext())
			{
				if (!model.CurrencyId1.HasValue || !model.CurrencyId2.HasValue || model.CurrencyId1 == model.CurrencyId2)
					return new WriterResult(false, $"Retarded TradePair warning.");

				var currency1 = await context.Currency
					.Where(x => x.Id == model.CurrencyId1.Value)
					.FirstOrDefaultNoLockAsync().ConfigureAwait(false);
				if (currency1 == null)
					return new WriterResult(false, $"Currency1 {model.CurrencyId1.Value} not found");

				var currency2 = await context.Currency
					.Where(x => x.Id == model.CurrencyId2.Value)
					.FirstOrDefaultNoLockAsync().ConfigureAwait(false);
				if (currency2 == null)
					return new WriterResult(false, $"Currency2 {model.CurrencyId1.Value} not found");

				var tradePair = await context.TradePair
					.Include(x => x.Currency1)
					.Include(x => x.Currency2)
					.Where(x => x.CurrencyId1 == model.CurrencyId1.Value && x.CurrencyId2 == model.CurrencyId2.Value)
					.FirstOrDefaultNoLockAsync().ConfigureAwait(false);
				if (tradePair != null)
					return new WriterResult(false,
						$"TradePair {tradePair.Currency1.Symbol}/{tradePair.Currency2.Symbol} already exists.");

				tradePair = await context.TradePair
					.Include(x => x.Currency1)
					.Include(x => x.Currency2)
					.Where(x => x.CurrencyId1 == model.CurrencyId2.Value && x.CurrencyId2 == model.CurrencyId1.Value)
					.FirstOrDefaultNoLockAsync().ConfigureAwait(false);
				if (tradePair != null)
					return new WriterResult(false,
						$"Inverse TradePair {tradePair.Currency1.Symbol}/{tradePair.Currency2.Symbol} already exists.");

				var newTradePair = new Entity.TradePair
				{
					CurrencyId1 = currency1.Id,
					CurrencyId2 = currency2.Id,
					Status = model.Status ?? Enums.TradePairStatus.OK,
					StatusMessage = model.StatusMessage,
					LastTrade = 0,
					Change = 0,
				};
				context.TradePair.Add(newTradePair);

                using (var dataContext = DataContextFactory.CreateContext())
                {
                    dataContext.LogActivity(adminUserId, $"Creating new Trade Pair: {newTradePair.Currency1.Symbol}_{newTradePair.Currency2.Symbol}");
                    await dataContext.SaveChangesAsync().ConfigureAwait(false);
                }

                await context.SaveChangesAsync().ConfigureAwait(false);
				await CacheService.InvalidateAsync(CacheKey.AllTradePairs(), CacheKey.TradePairs()).ConfigureAwait(false);
				return new WriterResult(true, $"Successfully created new tradepair {currency1.Symbol}/{currency2.Symbol}.");
			}
		}

		public async Task<IWriterResult> UpdateTradePair(string adminUserId, UpdateTradePairModel model)
		{
			using (var context = ExchangeDataContextFactory.CreateContext())
			{
				var tradePair = await context.TradePair
					.Include(x => x.Currency1)
					.Include(x => x.Currency2)
					.Where(x => x.Id == model.Id)
					.FirstOrDefaultNoLockAsync().ConfigureAwait(false);
				if (tradePair == null)
					return new WriterResult(false, $"TradePair {model.Id} not found.");

                var oldStatus = tradePair.Status;
				tradePair.Status = model.Status;
				tradePair.StatusMessage = model.StatusMessage;

                using (var dataContext = DataContextFactory.CreateContext())
                {
                    dataContext.LogActivity(adminUserId, $"Updating Trade Pair:{tradePair.Currency1.Symbol}_{tradePair.Currency2.Symbol} Old Status: {oldStatus}, New status {tradePair.Status}.");
                    await dataContext.SaveChangesAsync().ConfigureAwait(false);
                }

                await context.SaveChangesAsync().ConfigureAwait(false);
				await CacheService.InvalidateAsync(CacheKey.AllTradePairs(), CacheKey.TradePairs()).ConfigureAwait(false);
				return new WriterResult(true, $"Successfully updated tradepair.");
			}
		}
	}
}
