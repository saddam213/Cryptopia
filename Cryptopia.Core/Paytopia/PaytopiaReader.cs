using Cryptopia.Base;
using Cryptopia.Base.Extensions;
using Cryptopia.Common.Cache;
using Cryptopia.Common.Currency;
using Cryptopia.Infrastructure.Common.DataContext;
using Cryptopia.Infrastructure.Common.DataTables;
using Cryptopia.Common.Paytopia;
using Cryptopia.Common.Pool;
using Cryptopia.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptopia.Core.Paytopia
{
	public class PaytopiaReader : IPaytopiaReader
	{
		public IPoolReader PoolReader { get; set; }
		public ICurrencyReader CurrencyReader { get; set; }
		public ICacheService CacheService { get; set; }
		public IDataContextFactory DataContextFactory { get; set; }

		public async Task<PaytopiaItemModel> GetItem(PaytopiaItemType itemType)
		{
			var cacheResult = await CacheService.GetOrSetHybridAsync(CacheKey.PaytopiaItem(itemType), TimeSpan.FromMinutes(10), async () =>
			{
				using (var context = DataContextFactory.CreateReadOnlyContext())
				{
					var item = await context.PaytopiaItems
						.AsNoTracking()
						.Where(x => x.Type == itemType && x.IsEnabled)
						.Select(x => new PaytopiaItemModel
						{
							Id = x.Id,
							Name = x.Name,
							Description = x.Description,
							Price = x.Price,
							Type = x.Type,
							Category = x.Category,
							CurrencyId = x.CurrencyId
						}).FirstOrDefaultNoLockAsync().ConfigureAwait(false);

					var currency = await CurrencyReader.GetCurrency(item.CurrencyId).ConfigureAwait(false);
					if (currency != null)
						item.Symbol = currency.Symbol;
					return item;
				}
			}).ConfigureAwait(false);
			return cacheResult;
		}

		public async Task<List<PaytopiaItemModel>> GetItems()
		{
			var cacheResult = await CacheService.GetOrSetHybridAsync(CacheKey.PaytopiaItems(), TimeSpan.FromMinutes(10), async () =>
			{
				using (var context = DataContextFactory.CreateReadOnlyContext())
				{
					var items = await context.PaytopiaItems
						.AsNoTracking()
						.Where(x => x.IsEnabled)
						.Select(x => new PaytopiaItemModel
						{
							Id = x.Id,
							Name = x.Name,
							Description = x.Description,
							Price = x.Price,
							Type = x.Type,
							Category = x.Category,
							CurrencyId = x.CurrencyId
						}).ToListNoLockAsync().ConfigureAwait(false);

					foreach (var item in items)
					{
						var currency = await CurrencyReader.GetCurrency(item.CurrencyId).ConfigureAwait(false);
						if (currency != null)
							item.Symbol = currency.Symbol;
					}
					return items;
				}
			}).ConfigureAwait(false);
			return cacheResult;
		}

		public async Task<PaytopiaPaymentModel> AdminGetPayment(int id)
		{
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				var item = await context.PaytopiaPayments
					.AsNoTracking()
					.Where(x => x.Id == id)
					.Select(payment => new PaytopiaPaymentModel
					{
						Id = payment.Id,
						Type = payment.PaytopiaItem.Type,
						CurrencyId = payment.PaytopiaItem.CurrencyId,
						Amount = payment.Amount,
						Status = payment.Status,
						UserName = payment.User.UserName,
						IsAnonymous = payment.IsAnonymous,
						Begins = payment.Begins,
						Ends = payment.Ends,
						Timestamp = payment.Timestamp,
						TransferId = payment.TransferId,
						RefundId = payment.RefundId,
						ReferenceCode = payment.ReferenceCode,
						ReferenceId = payment.ReferenceId,
						RefundReason = payment.RefundReason,
						RequestData = payment.RequestData,
					}).FirstOrDefaultNoLockAsync().ConfigureAwait(false);

				var currency = await CurrencyReader.GetCurrency(item.CurrencyId).ConfigureAwait(false);
				item.Symbol = currency.Symbol;

				if (item.ReferenceId > 0)
				{
					if (item.Type == PaytopiaItemType.FeaturedCurrency || item.Type == PaytopiaItemType.LottoSlot || item.Type == PaytopiaItemType.RewardSlot || item.Type == PaytopiaItemType.TipSlot)
					{
						var refCurrency = await CurrencyReader.GetCurrency(item.ReferenceId).ConfigureAwait(false);
						if (refCurrency != null)
						{
							item.ReferenceName = refCurrency.Name;
							item.ReferenceAlgo = refCurrency.AlgoType;
							item.ReferenceSymbol = refCurrency.Symbol;
						}
					}
					else if (item.Type == PaytopiaItemType.FeaturedPool || item.Type == PaytopiaItemType.PoolListing)
					{
						var refPool = await PoolReader.GetPool(item.ReferenceId).ConfigureAwait(false);
						if (refPool != null)
						{
							item.ReferenceName = refPool.Name;
							item.ReferenceAlgo = refPool.AlgoType;
							item.ReferenceSymbol = refPool.Symbol;
						}
					}
				}

				return item;
			}
		}

		public async Task<DataTablesResponse> AdminGetPayments(DataTablesModel model)
		{
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				var query = context.PaytopiaPayments
					.AsNoTracking()
					.Select(payment => new PaymentDatatableModel
					{
						Id = payment.Id,
						Type = payment.PaytopiaItem.Type,
						Symbol = payment.PaytopiaItem.CurrencyId.ToString(),
						CurrencyId = payment.PaytopiaItem.CurrencyId,
						Amount = payment.Amount,
						Status = payment.Status,
						UserName = payment.User.UserName,
						IsAnonymous = payment.IsAnonymous,
						Begins = payment.Begins,
						Ends = payment.Ends,
						Timestamp = payment.Timestamp,
						TransferId = payment.TransferId,
						RefundId = payment.RefundId
					});

				return await query.GetDataTableResultNoLockAsync(model).ConfigureAwait(false);
			}
		}

		public async Task<DataTablesResponse> GetPayments(string userId, DataTablesModel model)
		{
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				var query = context.PaytopiaPayments
					.AsNoTracking()
					.Where(x => x.UserId == userId)
					.Select(payment => new PaymentDatatableModel
					{
						Id = payment.Id,
						Type = payment.PaytopiaItem.Type,
						CurrencyId = payment.PaytopiaItem.CurrencyId,
						Amount = payment.Amount,
						Status = payment.Status,
						UserName = payment.User.UserName,
						IsAnonymous = payment.IsAnonymous,
						Begins = payment.Begins,
						Ends = payment.Ends,
						Timestamp = payment.Timestamp,
						TransferId = payment.TransferId,
						RefundId = payment.RefundId,
					});

				var currencies = await CurrencyReader.GetCurrencies().ConfigureAwait(false);
				var symbolMap = currencies.ToDictionary(k => k.CurrencyId, v => v.Symbol);
				return await query.GetDataTableResultNoLockAsync(model, (item) =>
				{
					item.Symbol = symbolMap[item.CurrencyId];
				}).ConfigureAwait(false);
			}
		}

		public async Task<PaytopiaPaymentModel> GetPayment(string userId, int id)
		{
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				var item = await context.PaytopiaPayments
					.AsNoTracking()
					.Where(x => x.Id == id && x.UserId == userId)
					.Select(payment => new PaytopiaPaymentModel
					{
						Id = payment.Id,
						Type = payment.PaytopiaItem.Type,
						CurrencyId = payment.PaytopiaItem.CurrencyId,
						Amount = payment.Amount,
						Status = payment.Status,
						UserName = payment.User.UserName,
						IsAnonymous = payment.IsAnonymous,
						Begins = payment.Begins,
						Ends = payment.Ends,
						Timestamp = payment.Timestamp,
						TransferId = payment.TransferId,
						RefundId = payment.RefundId,
						ReferenceCode = payment.ReferenceCode,
						ReferenceId = payment.ReferenceId,
						RefundReason = payment.RefundReason,
						RequestData = payment.RequestData,
					}).FirstOrDefaultNoLockAsync().ConfigureAwait(false);

				var currency = await CurrencyReader.GetCurrency(item.CurrencyId).ConfigureAwait(false);
				item.Symbol = currency.Symbol;

				if (item.ReferenceId > 0)
				{
					if (item.Type == PaytopiaItemType.FeaturedCurrency || item.Type == PaytopiaItemType.LottoSlot || item.Type == PaytopiaItemType.RewardSlot || item.Type == PaytopiaItemType.TipSlot)
					{
						var refCurrency = await CurrencyReader.GetCurrency(item.ReferenceId).ConfigureAwait(false);
						if (refCurrency != null)
						{
							item.ReferenceName = refCurrency.Name;
							item.ReferenceAlgo = refCurrency.AlgoType;
							item.ReferenceSymbol = refCurrency.Symbol;
						}
					}
					else if (item.Type == PaytopiaItemType.FeaturedPool || item.Type == PaytopiaItemType.PoolListing)
					{
						var refPool = await PoolReader.GetPool(item.ReferenceId).ConfigureAwait(false);
						if (refPool != null)
						{
							item.ReferenceName = refPool.Name;
							item.ReferenceAlgo = refPool.AlgoType;
							item.ReferenceSymbol = refPool.Symbol;
						}
					}
				}

				return item;
			}
		}

		public async Task<List<FeaturedSlotItemModel>> GetFeaturedCurrencySlotItems()
		{
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				var slotDetail = await context.PaytopiaPayments
					.AsNoTracking()
					.Where(x => x.PaytopiaItem.Type == PaytopiaItemType.FeaturedCurrency && x.Ends > DateTime.UtcNow)
					.GroupBy(x => x.ReferenceId)
					.ToListNoLockAsync().ConfigureAwait(false);

				var currencies = await CurrencyReader.GetCurrencies().ConfigureAwait(false);
				var results = new List<FeaturedSlotItemModel>();
				foreach (var currency in currencies)
				{
					var isActive = false;
					var slotSummary = string.Empty;
					var existingSlotDetail = slotDetail.FirstOrDefault(x => x.Key == currency.CurrencyId);
					if (existingSlotDetail != null && existingSlotDetail.Any())
					{
						isActive = existingSlotDetail.Any(x => x.Begins < DateTime.UtcNow.Date && DateTime.UtcNow.Date < x.Ends);
						slotSummary = string.Join(",", existingSlotDetail.Select(x => $"Week: {x.Begins.WeekOfYear()} ({x.Begins.ToString("dd/MM/yyyy")} - {x.Ends.ToString("dd/MM/yyyy")})"));
					}

					var nextFreeSlot = PaytopiaWriter.GetNextFreeSlot(currency.CurrencyId, slotDetail.FirstOrDefault(x => x.Key == currency.CurrencyId));
					results.Add(new FeaturedSlotItemModel
					{
						Id = currency.CurrencyId,
						Name = currency.Symbol,
						IsFeatured = isActive,
						NextSlotWeek = nextFreeSlot.Begin.WeekOfYear(),
						NextSlotBegin = nextFreeSlot.Begin,
						NextSlotEnd = nextFreeSlot.End,
						SlotSummary = slotSummary
					});
				}
				return results;
			}
		}


		public async Task<List<FeaturedSlotItemModel>> GetFeaturedPoolSlotItems()
		{
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				var slotDetail = await context.PaytopiaPayments
						.AsNoTracking()
						.Where(x => x.PaytopiaItem.Type == PaytopiaItemType.FeaturedPool && x.Ends > DateTime.UtcNow)
						.GroupBy(x => x.ReferenceId)
						.ToListNoLockAsync().ConfigureAwait(false);

				var pools = await PoolReader.GetPools().ConfigureAwait(false);
				var results = new List<FeaturedSlotItemModel>();
				foreach (var pool in pools.DistinctBy(x => x.Symbol))
				{
					var isActive = false;
					var slotSummary = string.Empty;
					var existingSlotDetail = slotDetail.FirstOrDefault(x => x.Key == pool.Id);
					if (existingSlotDetail != null && existingSlotDetail.Any())
					{
						isActive = existingSlotDetail.Any(x => x.Begins < DateTime.UtcNow.Date && DateTime.UtcNow.Date < x.Ends);
						slotSummary = string.Join(",", existingSlotDetail.Select(x => $"Week: {x.Begins.WeekOfYear()} ({x.Begins.ToString("dd/MM/yyyy")} - {x.Ends.ToString("dd/MM/yyyy")})"));
					}

					var nextFreeSlot = PaytopiaWriter.GetNextFreeSlot(pool.Id, slotDetail.FirstOrDefault(x => x.Key == pool.Id));
					results.Add(new FeaturedSlotItemModel
					{
						Id = pool.Id,
						Name = pool.Symbol,
						IsFeatured = isActive,
						NextSlotWeek = nextFreeSlot.Begin.WeekOfYear(),
						NextSlotBegin = nextFreeSlot.Begin,
						NextSlotEnd = nextFreeSlot.End,
						SlotSummary = slotSummary
					});
				}
				return results;
			}
		}

		public async Task<List<LottoSlotItemModel>> GetLottoSlotItems()
		{
			var currencies = await CurrencyReader.GetCurrencies().ConfigureAwait(false);
			return currencies.Select(x => new LottoSlotItemModel
			{
				Id = x.CurrencyId,
				Name = x.Symbol
			})
			.OrderBy(x => x.Name)
			.ToList();
		}

		public async Task<List<RewardSlotItemModel>> GetRewardSlotItems()
		{
			var currencies = await CurrencyReader.GetCurrencies().ConfigureAwait(false);
			return currencies.Select(x => new RewardSlotItemModel
			{
				Id = x.CurrencyId,
				Name = x.Symbol,
				IsExpired = DateTime.UtcNow > x.RewardsExpires,
				Expires = x.RewardsExpires > DateTime.UtcNow
					? x.RewardsExpires
					: DateTime.UtcNow,
				NewExpiry = x.RewardsExpires > DateTime.UtcNow
					? x.RewardsExpires.AddDays(30)
					: DateTime.UtcNow.AddDays(30)
			})
			.OrderBy(x => x.Name)
			.ToList();
		}

		public async Task<List<TipSlotItemModel>> GetTipSlotItems()
		{
			var currencies = await CurrencyReader.GetCurrencies().ConfigureAwait(false);
			return currencies.Select(x => new TipSlotItemModel
			{
				Id = x.CurrencyId,
				Name = x.Symbol,
				IsExpired = DateTime.UtcNow > x.TippingExpires,
				Expires = x.TippingExpires > DateTime.UtcNow
					? x.TippingExpires
					: DateTime.UtcNow,
				NewExpiry = x.TippingExpires > DateTime.UtcNow
					? x.TippingExpires.AddDays(30)
					: DateTime.UtcNow.AddDays(30)
			})
			.OrderBy(x => x.Name)
			.ToList();
		}

		public async Task<List<PoolListingItemModel>> GetPoolListingItems()
		{
			var pools = await PoolReader.GetPools().ConfigureAwait(false);
			return pools.Select(x => new PoolListingItemModel
			{
				Id = x.Id,
				Name = $"{x.Symbol} ({x.AlgoType})"
			})
			.OrderBy(x => x.Name)
			.ToList();
		}

		public Task<List<string>> GetFlairItems()
		{
			return Task.FromResult(new List<string>
			{
				"bank",
				"barchart",
				"barchartalt",
				"barcode",
				"basecamp",
				"basketball",
				"bat",
				"batman",
			});
		}
	}
}
