using System;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Cryptopia.Base.Logging;
using Cryptopia.Common.DataContext;
using Cryptopia.Data.DataContext;
using Cryptopia.Enums;
using System.Collections.Generic;
using Cryptopia.Base.Extensions;
using Cryptopia.Infrastructure.Common.DataContext;

namespace Cryptopia.IntegrationService.Implementation
{
	public class PaytopiaProcessor : ProcessorBase<CancellationToken>
    {
        #region Private Fields

        private readonly Log _log = LoggingManager.GetLog(typeof(PaytopiaProcessor));
        private int _pollPeriod = 10;
		private CancellationToken _cancelToken;

        #endregion

        #region Private Properties

        private IDataContextFactory DataContextFactory { get; set; }
        private IPoolDataContextFactory PoolDataContextFactory { get; set; }
        private IExchangeDataContextFactory ExchangeDataContextFactory { get; set; }

        #endregion

        public PaytopiaProcessor(CancellationToken cancelToken) : base(cancelToken)
        {
#if DEBUG
			_pollPeriod = 1;
#endif
			_cancelToken = cancelToken;
			DataContextFactory = new DataContextFactory();
			PoolDataContextFactory = new PoolDataContextFactory();
			ExchangeDataContextFactory = new ExchangeDataContextFactory();
		}

        #region ProcessorBase Implementation

        protected override Log Log
        {
            get
            {
                return _log;
            }
        }

        public override string StartLog => "[Start] - Starting Paytopia processor.";
        public override string StopLog => "[Start] - Stopping Paytopia processor.";

		protected override async void Process()
		{
			Log.Message(LogLevel.Info, "[Process] - Starting Paytopia processor.");
			while (_isEnabled)
			{
				try
				{
					await ProcessPaytopiaItems().ConfigureAwait(false);
					await Task.Delay(TimeSpan.FromMinutes(_pollPeriod), _cancelToken).ConfigureAwait(false);
				}
				catch (TaskCanceledException)
				{
					Log.Message(LogLevel.Info, "[Process] - Paytopia processing canceled");
					break;
				}
			}
			_isRunning = false;
			Log.Message(LogLevel.Info, "[Process] - Stopped Paytopia processor.");
		}

        #endregion

        #region Private Methods

        private async Task ProcessPaytopiaItems()
		{
			try
			{
				var now = DateTime.UtcNow;
				var beginingOfWeek = now.StartOfWeek(DayOfWeek.Monday);
				var endOfWeek = beginingOfWeek.AddDays(7);
				var featuredItems = new List<FeaturedInfo>();
				using (var context = DataContextFactory.CreateContext())
				{
					featuredItems = await context.PaytopiaPayments
						.Where(x => x.Begins >= beginingOfWeek && x.Ends <= endOfWeek && (x.PaytopiaItem.Type == PaytopiaItemType.FeaturedCurrency || x.PaytopiaItem.Type == PaytopiaItemType.FeaturedPool))
						.Select(x => new FeaturedInfo
						{
							Id = x.ReferenceId,
							Type = x.PaytopiaItem.Type,
							ExpireTime = x.Ends
						}).ToListNoLockAsync();
				}

				if (!featuredItems.Any())
					return;

				// update currencies
				using (var context = ExchangeDataContextFactory.CreateContext())
				{
					var currencyIds = featuredItems.Where(x => x.Type == PaytopiaItemType.FeaturedCurrency).Select(x => x.Id).ToList();
					var currencies = await context.Currency.Where(x => currencyIds.Contains(x.Id)).ToListNoLockAsync();
					foreach (var currency in currencies)
					{
						var featureExpireTime = featuredItems.First(x => x.Type == PaytopiaItemType.FeaturedCurrency && x.Id == currency.Id).ExpireTime;
						if (featureExpireTime != currency.FeaturedExpires)
							currency.FeaturedExpires = featureExpireTime;
					}
					await context.SaveChangesAsync();
				}

				// update pools
				using (var context = PoolDataContextFactory.CreateContext())
				{
					var poolIds = featuredItems.Where(x => x.Type == PaytopiaItemType.FeaturedPool).Select(x => x.Id).ToList();
					var pools = await context.Pool.Where(x => poolIds.Contains(x.Id)).ToListNoLockAsync();
					foreach (var pool in pools)
					{
						var featureExpireTime = featuredItems.First(x => x.Type == PaytopiaItemType.FeaturedPool && x.Id == pool.Id).ExpireTime;
						if (featureExpireTime != pool.FeaturedExpires)
							pool.FeaturedExpires = featureExpireTime;
					}
					await context.SaveChangesAsync();
				}
			}
			catch (Exception ex)
			{
				Log.Exception("[ProcessPaytopiaItems] - An exception occurred processing Paytopia items.", ex);
			}
		}

        #endregion

        public class FeaturedInfo
		{
			public DateTime ExpireTime { get; internal set; }
			public int Id { get; set; }
			public PaytopiaItemType Type { get; set; }
		}
	}
}