using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.Reflection;
using System.Threading;
using Cryptopia.Enums;
using Cryptopia.Base.Logging;
using Cryptopia.Common.DataContext;
using Cryptopia.Data.DataContext;
using Cryptopia.Base;
using Cryptopia.Entity;
using Cryptopia.Infrastructure.Common.DataContext;

namespace Cryptopia.IntegrationService.Implementation
{
	public class ExchangeProcessor : ProcessorBase<CancellationToken>
    {
        private readonly Log _log = LoggingManager.GetLog(typeof(ExchangeProcessor));
        private List<IExchange> _exchanges = new List<IExchange>();
		private int _pollPeriod = 300;
		private int _expirePeriod = 15;
		private CancellationToken _cancelToken;

        private IExchangeDataContextFactory ExchangeDataContextFactory { get; set; }

        public ExchangeProcessor(CancellationToken cancelToken) : base(cancelToken)
        {
			_cancelToken = cancelToken;
			ExchangeDataContextFactory = new ExchangeDataContextFactory();
			_exchanges.AddRange(Assembly.GetExecutingAssembly()
				.GetTypes()
				.Where(x => x.IsClass && typeof (IExchange).IsAssignableFrom(x))
				.Select(x => Activator.CreateInstance(x) as IExchange));
		}

        protected override Log Log
        {
            get
            {
                return _log;
            }
        }

        public override string StartLog => "[Start] - Starting Exchange processor.";
        public override string StopLog => "[Start] - Stopping Exchange processor.";

		protected override async void Process()
		{
			Log.Message(LogLevel.Info, "[Process] - Starting processor.");
			while (_isEnabled)
			{
				try
				{
					Log.Message(LogLevel.Info, "[Process] - Starting process...");
					var startTime = DateTime.UtcNow;
					await ProcessExchanges().ConfigureAwait(false);
					var elapsed = DateTime.UtcNow - startTime;
					Log.Message(LogLevel.Info, "[Process] - Process completed, Elapsed: {0}", elapsed);

					var delay = _pollPeriod - elapsed.TotalSeconds;
					if (delay > 0)
					{
						Log.Message(LogLevel.Info, "[Process] - Waiting {0} seconds...", delay);
						await Task.Delay(TimeSpan.FromSeconds(delay), _cancelToken).ConfigureAwait(false);
					}
				}
				catch (TaskCanceledException)
				{
					Log.Message(LogLevel.Info, "[Process] - Processing canceled");
					break;
				}
			}
			_isRunning = false;
			Log.Message(LogLevel.Info, "[Process] - Stopped processor.");
		}

		public async Task ProcessExchanges()
		{
			using (var context = ExchangeDataContextFactory.CreateContext())
			{
				Log.Message(LogLevel.Info, "[ProcessExchanges] - Creating tradepair map...");
				var tradePairs = await context.TradePair
					.Where(x => x.Status == TradePairStatus.OK || x.Status == TradePairStatus.Paused)
					.Select(t => new
					{
						TradePairId = t.Id,
						Symbol = t.Currency1.Symbol,
						BaseSymbol = t.Currency2.Symbol
					}).ToListNoLockAsync();
				if (tradePairs.IsNullOrEmpty())
				{
					return;
				}
				var tradePairMap = tradePairs.ToDictionary(k => string.Format("{0}_{1}", k.Symbol, k.BaseSymbol), v => v.TradePairId);
				Log.Message(LogLevel.Info, "[ProcessExchanges] - Tradepair map created.");

				Log.Message(LogLevel.Info, "[ProcessExchanges] - Processing exchanges...");
				foreach (var exchange in _exchanges)
				{
					if (!_isEnabled)
					{
						break;
					}

					try
					{
						Log.Message(LogLevel.Info, "[ProcessExchanges] - Processing exchange. Exchange: {0}", exchange.Name);
						var existingData =
							await context.IntegrationMarketData.Where(x => x.IntegrationExchangeId == exchange.Id).ToListNoLockAsync();
						var marketData = await exchange.GetMarketData(tradePairMap);
						if (!marketData.IsNullOrEmpty())
						{
							Log.Message(LogLevel.Info, "[ProcessExchanges] - Market data found, Updating database");
							foreach (var data in marketData)
							{
								if (!_isEnabled)
								{
									break;
								}

								if (tradePairMap.ContainsKey(data.TradePair))
								{
									var existing = existingData.FirstOrDefault(x => x.TradePairId == tradePairMap[data.TradePair]);
									if (existing == null)
									{
										existing = new IntegrationMarketData
										{
											IntegrationExchangeId = exchange.Id,
											TradePairId = tradePairMap[data.TradePair]
										};
										context.IntegrationMarketData.Add(existing);
									}

									existing.Timestamp = DateTime.UtcNow;
									existing.Ask = data.Ask;
									existing.Bid = data.Bid;
									existing.Last = data.Last;
									existing.Volume = data.Volume;
									existing.BaseVolume = data.BaseVolume;
									existing.MarketUrl = data.MarketUrl;
								}
							}
							await context.SaveChangesAsync();
							Log.Message(LogLevel.Info, "[ProcessExchanges] - Market data updated.");
						}

						var expiredData = existingData.Where(x => x.Timestamp.AddMinutes(_expirePeriod) < DateTime.UtcNow);
						if (expiredData.Any())
						{
							Log.Message(LogLevel.Info, "[ProcessExchanges] - Expired data found, Deleteing...");
							foreach (var expired in expiredData)
							{
								if (!_isEnabled)
								{
									break;
								}

								context.IntegrationMarketData.Remove(expired);
							}
							await context.SaveChangesAsync();
							Log.Message(LogLevel.Info, "[ProcessExchanges] - Expired data deleted.");
						}
					}
					catch (Exception ex)
					{
						Log.Exception("[ProcessExchanges] - An exception occured processing exchange, Exchange: {0}", ex, exchange.Name);
					}
				}
				Log.Message(LogLevel.Info, "[ProcessExchanges] - Processing exchanges complete.");
			}
		}
	}
}