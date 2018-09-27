using System;
using System.Linq;
using System.Threading.Tasks;
using Cryptopia.Common.Currency;
using Cryptopia.Infrastructure.Common.DataContext;
using Cryptopia.Common.Cache;
using Cryptopia.Common.DataContext;
using Cryptopia.Infrastructure.Common.Results;

namespace Cryptopia.Core.Currency
{
	public class CurrencyWriter : ICurrencyWriter
	{
		public ICacheService CacheService { get; set; }
		public IExchangeDataContextFactory ExchangeDataContextFactory { get; set; }

		public async Task<IWriterResult> UpdateCurrency(UpdateCurrencyModel model)
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

					await context.SaveChangesAsync().ConfigureAwait(false);
					await
						CacheService.InvalidateAsync(CacheKey.Currencies(), CacheKey.CurrencyInfo(), CacheKey.CurrencyDataTable(),
							CacheKey.CurrencySummary(model.Id)).ConfigureAwait(false);
					return new WriterResult(true, "Succesfully updated currency settings.");
				}
			}
			catch (Exception)
			{
				return null;
			}
		}

		public async Task<IWriterResult> UpdateCurrencyInfo(UpdateCurrencyInfoModel model)
		{
			try
			{
				using (var context = ExchangeDataContextFactory.CreateContext())
				{
					var currencyInfo =
						await context.CurrencyInfo.Where(c => c.Id == model.Id).FirstOrDefaultNoLockAsync().ConfigureAwait(false);
					if (currencyInfo == null)
						return new WriterResult(false, "Currency not found");

					currencyInfo.AlgoType = model.AlgoType;
					currencyInfo.BlockExplorer = model.BlockExplorer;
					currencyInfo.BlockReward = model.BlockReward;
					currencyInfo.BlockTime = model.BlockTime;
					currencyInfo.CryptopiaForum = model.CryptopiaForum;
					currencyInfo.Description = model.Summary;
					currencyInfo.DiffRetarget = model.DiffRetarget;
					currencyInfo.LaunchForum = model.LaunchForum;
					currencyInfo.MaxStakeAge = model.MaxStakeAge;
					currencyInfo.MinStakeAge = model.MinStakeAge;
					currencyInfo.NetworkType = model.NetworkType;
					currencyInfo.PosRate = model.PosRate;
					currencyInfo.Source = model.Source;
					currencyInfo.TotalCoin = model.TotalCoin;
					currencyInfo.TotalPremine = model.TotalPremine;
					currencyInfo.WalletLinux = model.WalletLinux;
					currencyInfo.WalletMac = model.WalletMac;
					currencyInfo.WalletMobile = model.WalletMobile;
					currencyInfo.WalletWeb = model.WalletWeb;
					currencyInfo.WalletWindows = model.WalletWindows;
					currencyInfo.Website = model.Website;
					currencyInfo.LastUpdated = DateTime.UtcNow;

					var ratingInfo = CalulateRating(model, currencyInfo.MaxRating);
					currencyInfo.TotalRating = ratingInfo.TotalRating;
					currencyInfo.StarRating = ratingInfo.StarRating;

					await context.SaveChangesAsync().ConfigureAwait(false);
					await
						CacheService.InvalidateAsync(CacheKey.CurrencyInfo(), CacheKey.CurrencyDataTable(),
							CacheKey.CurrencySummary(model.Id)).ConfigureAwait(false);
					return new WriterResult(true, "Succesfully updated currency details.");
				}
			}
			catch (Exception)
			{
				return null;
			}
		}

		private RatingInfo CalulateRating(UpdateCurrencyInfoModel model, int maxRating)
		{
			var rating = maxRating; // 1000
			if (!model.WalletWindows)
				rating -= 100;
			if (!model.WalletLinux)
				rating -= 100;
			if (!model.WalletMac)
				rating -= 100;
			if (!model.WalletMobile)
				rating -= 100;
			if (!model.WalletWeb)
				rating -= 100;
			if (string.IsNullOrEmpty(model.Website))
				rating -= 100;
			if (string.IsNullOrEmpty(model.BlockExplorer))
				rating -= 200;
			if (string.IsNullOrEmpty(model.CryptopiaForum))
				rating -= 200;
			if (model.TotalPremine > 0)
			{
				if (model.TotalPremine <= 2)
					rating -= 200;
				else if (model.TotalPremine <= 10)
					rating -= 300;
				else if (model.TotalPremine <= 50)
					rating -= 400;
				else if (model.TotalPremine > 50)
					rating -= 600;
			}

			rating = Math.Max(0, rating);

			var starRating = Math.Round(((5.0/100.0)*((double) rating/maxRating*100.0))*2, MidpointRounding.AwayFromZero)/2;
			return new RatingInfo
			{
				StarRating = starRating,
				TotalRating = Math.Max(0, rating),
			};
		}

		public class RatingInfo
		{
			public int MaxRating { get; set; }
			public int TotalRating { get; set; }
			public double StarRating { get; set; }
		}
	}
}