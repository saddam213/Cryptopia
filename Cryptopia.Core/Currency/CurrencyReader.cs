using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cryptopia.Common.Currency;
using Cryptopia.Infrastructure.Common.DataContext;
using Cryptopia.Infrastructure.Common.DataTables;
using Cryptopia.Common.Cache;
using Cryptopia.Common.Deposit;
using System.Net.Http;
using Cryptopia.Common.DataContext;
using Newtonsoft.Json.Linq;

namespace Cryptopia.Core.Currency
{
	public class CurrencyReader : ICurrencyReader
	{
		public ICacheService CacheService { get; set; }
		public IDepositService DepositService { get; set; }
		public IExchangeDataContextFactory ExchangeDataContextFactory { get; set; }

		public async Task<DataTablesResponse> GetCurrencyDataTable(DataTablesModel model)
		{
			using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
			{
				var query = context.Currency
					.AsNoTracking()
					.Where(c => c.IsEnabled)
					.Select(currency => new
					{
						Id = currency.Id,
						Symbol = currency.Symbol,
						Currency = currency.Name,
						Status = currency.Status,
						StatusMessage = currency.StatusMessage,
						ListingStatus = currency.ListingStatus,
						//FeaturedExpires = currency.FeaturedExpires,
						//RewardsExpires = currency.RewardsExpires,
						//TippingExpires = currency.TippingExpires
					});

				return await query.GetDataTableResultNoLockAsync(model).ConfigureAwait(false);
			}
		}

		public async Task<UpdateCurrencyModel> GetUpdateCurrency(int currencyId)
		{
			using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
			{
				return await context.Currency
					.AsNoTracking()
					.Where(c => c.Id == currencyId)
					.Select(currency => new UpdateCurrencyModel
					{
						Id = currency.Id,
						Name = currency.Name,
						Symbol = currency.Symbol,
						PoolFee = currency.PoolFee,
						TradeFee = currency.TradeFee,
						WithdrawFee = currency.WithdrawFee,
						WithdrawFeeType = currency.WithdrawFeeType,
						WithdrawMin = currency.MinWithdraw,
						WithdrawMax = currency.MaxWithdraw,
						TipMin = currency.MinTip,
						MinBaseTrade = currency.MinBaseTrade,
						MinConfirmations = currency.MinConfirmations,
						Status = currency.Status,
						StatusMessage = currency.StatusMessage,
						ListingStatus = currency.ListingStatus,
					}).FirstOrDefaultNoLockAsync().ConfigureAwait(false);
			}
		}

		public async Task<UpdateCurrencyInfoModel> GetCurrencyInfo(int currencyId)
		{
			using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
			{
				return await context.CurrencyInfo
					.AsNoTracking()
					.Where(c => c.Id == currencyId)
					.Select(currency => new UpdateCurrencyInfoModel
					{
						Id = currency.Id,
						Name = currency.Currency.Name,
						Symbol = currency.Currency.Symbol,

						AlgoType = currency.AlgoType,
						BlockExplorer = currency.BlockExplorer,
						BlockReward = currency.BlockReward,
						BlockTime = currency.BlockTime,
						CryptopiaForum = currency.CryptopiaForum,
						Summary = currency.Description,
						DiffRetarget = currency.DiffRetarget,
						LaunchForum = currency.LaunchForum,
						MaxStakeAge = currency.MaxStakeAge,
						MinStakeAge = currency.MinStakeAge,
						NetworkType = currency.NetworkType,
						PosRate = currency.PosRate,
						Source = currency.Source,
						TotalCoin = currency.TotalCoin,
						TotalPremine = currency.TotalPremine,
						WalletLinux = currency.WalletLinux,
						WalletMac = currency.WalletMac,
						WalletMobile = currency.WalletMobile,
						WalletWeb = currency.WalletWeb,
						WalletWindows = currency.WalletWindows,
						Website = currency.Website
					}).FirstOrDefaultNoLockAsync().ConfigureAwait(false);
			}
		}

		public async Task<DataTablesResponse> GetCurrencyInfo(DataTablesModel model)
		{
			var cacheResult = await CacheService.GetOrSetHybridAsync(CacheKey.CurrencyInfo(), TimeSpan.FromMinutes(5), async () =>
			{
				using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
				{
					var query = context.Currency
						.AsNoTracking()
						.Where(c => c.IsEnabled)
						.Select(currency => new
						{
							Id = currency.Id,
							Name = currency.Name,
							Symbol = currency.Symbol,
							Rating = currency.Info.StarRating,
							Algo = currency.Info.AlgoType,
							Network = currency.Info.NetworkType,
							Connections = currency.Connections,
							BlockHeight = currency.Block,
							Status = currency.Status,
							StatusMessage = currency.StatusMessage,
							ListingStatus = currency.ListingStatus,
							Version = currency.Version,
						}).OrderBy(x => x.Name);

					return await query.GetDataTableResultNoLockAsync(model).ConfigureAwait(false);
				}
			}).ConfigureAwait(false);
			return cacheResult;
		}

		public async Task<CurrencySummaryModel> GetCurrencySummary(int currencyId)
		{
			var cacheResult = await CacheService.GetOrSetHybridAsync(CacheKey.CurrencySummary(currencyId), TimeSpan.FromMinutes(10), async () =>
			{
				using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
				{
					var summary = await context.CurrencyInfo
						.AsNoTracking()
						.Where(c => c.Id == currencyId)
						.Select(currency => new CurrencySummaryModel
						{
							Id = currency.Id,
							Name = currency.Currency.Name,
							Symbol = currency.Currency.Symbol,
							LastUpdated = currency.LastUpdated,
							StarRating = currency.StarRating,

							// Info
							AlgoType = currency.AlgoType,
							NetworkType = currency.NetworkType,
							CurrentBlock = currency.Currency.Block,
							BlockTime = currency.BlockTime,
							BlockReward = currency.BlockReward,
							TotalCoin = currency.TotalCoin,
							PosRate = currency.PosRate,
							MaxStakeAge = currency.MaxStakeAge,
							MinStakeAge = currency.MinStakeAge,
							DiffRetarget = currency.DiffRetarget,

							// Links
							Website = currency.Website,
							CryptopiaForum = currency.CryptopiaForum,
							LaunchForum = currency.LaunchForum,
							BlockExplorer = currency.BlockExplorer,
							Source = currency.Source,

							// Rating
							TotalRating = currency.TotalRating,
							MaxRating = currency.MaxRating,
							TotalPremine = currency.TotalPremine,
							WalletLinux = currency.WalletLinux,
							WalletMac = currency.WalletMac,
							WalletMobile = currency.WalletMobile,
							WalletWeb = currency.WalletWeb,
							WalletWindows = currency.WalletWindows,

							// Settings
							PoolFee = currency.Currency.PoolFee,
							TradeFee = currency.Currency.TradeFee,
							WithdrawFee = currency.Currency.WithdrawFee,
							WithdrawMin = currency.Currency.MinWithdraw,
							WithdrawMax = currency.Currency.MaxWithdraw,
							TipMin = currency.Currency.MinTip,
							MinConfirmations = currency.Currency.MinConfirmations,
							TippingExpires = currency.Currency.TippingExpires,
							RewardsExpires = currency.Currency.RewardsExpires,


							// Other
							WithdrawFeeType = currency.Currency.WithdrawFeeType,
							MinBaseTrade = currency.Currency.MinBaseTrade,
							Status = currency.Currency.Status,
							StatusMessage = currency.Currency.StatusMessage,
							ListingStatus = currency.Currency.ListingStatus,
							FeaturedExpires = currency.Currency.FeaturedExpires,
							Summary = currency.Description,
						}).FirstOrDefaultNoLockAsync().ConfigureAwait(false);

					return summary;
				}
			}).ConfigureAwait(false);
			return cacheResult;
		}

		public async Task<List<CurrencyModel>> GetCurrencies()
		{
			return await GetOrCacheCurrencies().ConfigureAwait(false);
		}

		public async Task<List<BaseCurrencyModel>> GetBaseCurrencies()
		{
			return await GetOrCacheBaseCurrencies().ConfigureAwait(false);
		}

		public async Task<CurrencyModel> GetCurrency(int currencyId)
		{
			return await GetCurrency(c => c.CurrencyId == currencyId).ConfigureAwait(false);
		}

		public async Task<CurrencyModel> GetCurrency(string symbol)
		{
			return await GetCurrency(c => c.Symbol == symbol).ConfigureAwait(false);
		}

		public async Task<CurrencyPeerInfoModel> GetPeerInfo(int currencyId)
		{
			return await GetOrCachePeerInfo(currencyId).ConfigureAwait(false);
		}


		private async Task<CurrencyModel> GetCurrency(Func<CurrencyModel, bool> selector)
		{
			var currencies = await GetOrCacheCurrencies().ConfigureAwait(false);
			if (currencies != null)
				return currencies.FirstOrDefault(selector);

			return null;
		}

		private async Task<List<CurrencyModel>> GetOrCacheCurrencies()
		{
			var cacheResult = await CacheService.GetOrSetHybridAsync(CacheKey.Currencies(), TimeSpan.FromMinutes(10), async () =>
			{
				using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
				{
					var summary = await context.Currency
						.AsNoTracking()
						.Where(t => t.IsEnabled)
						.Select(currency => new CurrencyModel
						{
							CurrencyId = currency.Id,
							Name = currency.Name,
							Symbol = currency.Symbol,
							Connections = currency.Connections,
							Status = currency.Status,
							StatusMessage = currency.StatusMessage,
							ListingStatus = currency.ListingStatus,
							Version = currency.Version,
							AlgoType = currency.Info.AlgoType,
							BaseAddress = currency.BaseAddress,
							Block = currency.Block,
							BlockTime = currency.Info.BlockTime,
							Errors = currency.Errors,
							FeaturedExpires = currency.FeaturedExpires,
							TippingExpires = currency.TippingExpires,
							RewardsExpires = currency.RewardsExpires,
							MinBaseTrade = currency.MinBaseTrade,
							MinConfirmations = currency.MinConfirmations,
							NetworkType = currency.Info.NetworkType,
							PoolFee = currency.PoolFee,
							Summary = currency.Info.Description,
							TradeFee = currency.TradeFee,
							Type = currency.Type,
							TipMin = currency.MinTip,
							WithdrawFee = currency.WithdrawFee,
							WithdrawFeeType = currency.WithdrawFeeType,
							WithdrawMax = currency.MaxWithdraw,
							WithdrawMin = currency.MinWithdraw,
							Website = currency.Info.Website,
							Rank = currency.Rank,

							QrFormat = currency.Settings.QrFormat,
							DepositInstructions = currency.Settings.DepositInstructions,
							DepositMessage = currency.Settings.DepositMessage,
							DepositMessageType = currency.Settings.DepositMessageType,
							WithdrawInstructions = currency.Settings.WithdrawInstructions,
							WithdrawMessage = currency.Settings.WithdrawMessage,
							WithdrawMessageType = currency.Settings.WithdrawMessageType,
							AddressType = currency.Settings.AddressType,
							CurrencyDecimals = currency.Settings.Decimals
						}).OrderBy(c => c.Name)
						.ToListNoLockAsync().ConfigureAwait(false);
					return summary;
				}
			}).ConfigureAwait(false);
			return cacheResult;
		}

		private async Task<List<BaseCurrencyModel>> GetOrCacheBaseCurrencies()
		{
			var cacheResult = await CacheService.GetOrSetHybridAsync(CacheKey.BaseCurrencies(), TimeSpan.FromMinutes(10), async () =>
			{
				using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
				{
					var query = context.TradePair
					.AsNoTracking()
					.Where(x => x.Status != Enums.TradePairStatus.Closed && x.Currency2.IsEnabled)
					.Select(currency => new BaseCurrencyModel
					{
						CurrencyId = currency.CurrencyId2,
						Name = currency.Currency2.Name,
						Symbol = currency.Currency2.Symbol,
						Rank = currency.Currency2.Rank
					}).Distinct()
					.OrderBy(c => c.CurrencyId);

					return await query.ToListNoLockAsync().ConfigureAwait(false);
				}
			}).ConfigureAwait(false);
			return cacheResult;
		}

		private async Task<CurrencyPeerInfoModel> GetOrCachePeerInfo(int currencyId)
		{
			var cacheResult = await CacheService.GetOrSetHybridAsync(CacheKey.CurrencyPeerInfo(currencyId), TimeSpan.FromMinutes(10), async () =>
			{

				var currency = await GetCurrency(currencyId).ConfigureAwait(false);
				var result = new CurrencyPeerInfoModel
				{
					Name = currency.Name,
					Symbol = currency.Symbol,
					PeerInfo = new List<PeerInfoModel>()
				};

				var peerData = await DepositService.GetPeerInfo(currencyId).ConfigureAwait(false);
#if DEBUG
				peerData.Add(new PeerInfoModel { Address = "116.74.107.102:9229" });
				peerData.Add(new PeerInfoModel { Address = "98.115.147.74:9229" });
				peerData.Add(new PeerInfoModel { Address = "45.35.64.39:9229" });
				peerData.Add(new PeerInfoModel { Address = "188.166.182.57:9229" });
#endif
				foreach (var peer in peerData)
				{
					try
					{
						var ipaddress = peer.Address;
						if (ipaddress.Contains(":"))
							ipaddress = ipaddress.Split(':')[0];

						using (var client = new HttpClient())
						{
							client.Timeout = TimeSpan.FromSeconds(5);
							var peerLocation = JObject.Parse(await client.GetStringAsync("http://gd.geobytes.com/GetCityDetails?fqcn=" + ipaddress).ConfigureAwait(false)).ToObject<PeerData>();
							if (peerLocation != null && peerLocation.geobyteslatitude.HasValue && peerLocation.geobyteslongitude.HasValue)
							{
								peer.geobyteslatitude = peerLocation.geobyteslatitude;
								peer.geobyteslongitude = peerLocation.geobyteslongitude;
							}
						}
					}
					catch { }
				}
				result.PeerInfo = peerData;
				return result;
			}).ConfigureAwait(false);
			return cacheResult;
		}

		public class PeerData
		{
			public double? geobyteslatitude { get; set; }
			public double? geobyteslongitude { get; set; }

			public bool IsValid
			{
				get { return geobyteslatitude.HasValue && geobyteslongitude.HasValue; }
			}
		}

	}
}