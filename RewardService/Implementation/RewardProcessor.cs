using Cryptopia.RewardService.TradeService;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cryptopia.Enums;
using Cryptopia.Base.Logging;
using Cryptopia.Base;
using Cryptopia.Common.DataContext;
using Cryptopia.Data.DataContext;
using Cryptopia.Entity;
using Cryptopia.Infrastructure.Common.DataContext;

namespace Cryptopia.RewardService.Implementation
{
	public class RewardProcessor
	{
		#region Fields

		private readonly Log Log = LoggingManager.GetLog(typeof(RewardProcessor));
		private bool _isEnabled;
		private bool _isRunning;
		private int _pollPeriod = 10;
		private CancellationToken _cancelToken;
		private Random _random = new Random();
		private Dictionary<RewardType, RewardResult> _lastValues = new Dictionary<RewardType, RewardResult>();
		private DateTime _nextGenerate = DateTime.MinValue;
		private Dictionary<DateTime, RewardItem> _rewardSchedule = new Dictionary<DateTime, RewardItem>();
		private List<RewardType> _rewardTypeWeightedList = new List<RewardType>();
		private int _rewardCurrencyCount = 0;
		private DateTime _lastRewardCurrencyCheck = DateTime.UtcNow.AddMinutes(10);

		#endregion

		#region Constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="RewardProcessor"/> class.
		/// </summary>
		/// <param name="pollPeriod">The poll period.</param>
		/// <param name="cancelToken">The cancel token.</param>
		public RewardProcessor(int pollPeriod, CancellationToken cancelToken)
		{
			_pollPeriod = pollPeriod;
			_cancelToken = cancelToken;
			ExchangeDataContextFactory = new ExchangeDataContextFactory();
			CreateRewardTypeWeightedList();
		}



		#endregion

		#region Properties

		public bool Running
		{
			get { return _isRunning; }
		}

		public IExchangeDataContextFactory ExchangeDataContextFactory { get; set; }

		#endregion

		#region Process

		/// <summary>
		/// Starts this instance.
		/// </summary>
		public void Start()
		{
			if (_isEnabled)
			{
				return;
			}

			Log.Message(LogLevel.Info, "[Start] - Starting processor.");
			_isRunning = true;
			_isEnabled = true;

			Process();
		}

		/// <summary>
		/// Stops this instance.
		/// </summary>
		public void Stop()
		{
			Log.Message(LogLevel.Info, "[Stop] - Stopping processor.");
			_isEnabled = false;
		}

		/// <summary>
		/// Processes this instance.
		/// </summary>
		private async void Process()
		{
			await GetLastRewardItems();
			while (_isEnabled)
			{
				try
				{
					await ProcessRewards().ConfigureAwait(false);

					// Check if any new currencies have been added to the rewardbot
					if(DateTime.UtcNow > _lastRewardCurrencyCheck)
					{
						Log.Message(LogLevel.Info, "[Process] - Checking for new reward currencies...");
						if (await IsRegenerationRequired())
						{
							Log.Message(LogLevel.Info, "[Process] - Reward currencies have changed, regenerating schedule...");
							await GenerateSchedule();
							Log.Message(LogLevel.Info, "[Process] - Regenerating schedule complete.");
						}
						_lastRewardCurrencyCheck = DateTime.UtcNow.AddMinutes(10);
						Log.Message(LogLevel.Info, "[Process] - Checking for new reward currencies complete.");
					}

					await Task.Delay(TimeSpan.FromSeconds(_pollPeriod), _cancelToken).ConfigureAwait(false);
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

		/// <summary>
		/// Processes the rewards.
		/// </summary>
		private async Task ProcessRewards()
		{
			try
			{
				if (DateTime.UtcNow > _nextGenerate)
				{
					Log.Message(LogLevel.Info, "[ProcessRewards] - GenerateTimes triggerd");
					await GenerateSchedule();
					_nextGenerate = DateTime.UtcNow.AddHours(24);
				}

				var dueItems = _rewardSchedule.Where(x => x.Key < DateTime.UtcNow);
				if (dueItems.IsNullOrEmpty())
				{
					Log.Message(LogLevel.Debug, "[ProcessRewards] - No rewards found to process");
					return;
				}

				var keysToRemove = new List<DateTime>(dueItems.Select(x => x.Key));
				var rewards = dueItems.Select(x => x.Value).GroupBy(x => x.RewardType);
				foreach (var item in rewards)
				{
					// If there is more that 1 reward type for this period, only process the biggest prize skip the rest.
					var rewarditem = item.MaxBy(x => x.Percent);
					Log.Message(LogLevel.Info, "[ProcessRewards] - Found {0} rewards for type, largest reward chosen: {1}%", item.Count(), rewarditem.Percent);
					await RewardUser(rewarditem);
				}

				foreach (var key in keysToRemove)
				{
					Log.Message(LogLevel.Debug, "[ProcessRewards] - Removing reward from schedule, Key: {0}", key);
					_rewardSchedule.Remove(key);
				}
			}
			catch (Exception ex)
			{
				Log.Exception("[ProcessRewards] - An exception occured processing rewards.", ex);
			}
		}

		#endregion

		#region Helpers

		/// <summary>
		/// Generates the schedule.
		/// </summary>
		private async Task GenerateSchedule()
		{
			try
			{
				Log.Message(LogLevel.Info, "[GenerateSchedule] - Generating reward schedule for the next 24 hours...");
				_rewardSchedule.Clear();
				var maxMilliSeconds = 86400000;
				var start = DateTime.UtcNow;
				using (var context = ExchangeDataContextFactory.CreateContext())
				{
					var balances = await context.Balance
						.Where(b => b.UserId == Constant.SYSTEM_USER_REWARD && b.Currency.RewardsExpires > DateTime.UtcNow)
						.Select(b => new
						{
							CurrencyId = b.CurrencyId,
							Symbol = b.Currency.Symbol,
							Available = b.Total - (b.PendingWithdraw + b.HeldForTrades + b.Unconfirmed)
						}).ToListNoLockAsync();
					if (balances.IsNullOrEmpty())
					{
						Log.Message(LogLevel.Error, "[GenerateSchedule] - Failed to fetch RewardBots balances.");
						return;
					}

				
					var availableBalances = balances.Where(x => x.Available > 0);
					if (availableBalances.IsNullOrEmpty())
					{
						Log.Message(LogLevel.Warn, "[GenerateSchedule] - All balances are empty.");
						return;
					}

					_rewardCurrencyCount = availableBalances.Count();
					var currencyIds = availableBalances.Select(x => x.CurrencyId);
					int randomCurrencyId = currencyIds.ElementAt(_random.Next(currencyIds.Count()));

					Log.Message(LogLevel.Info, "[GenerateSchedule] - {0} balances found to generate rewards for...", availableBalances.Count());
					foreach (var balance in availableBalances)
					{
						var avaliable = balance.Available;

						// 0.01%
						for (int i = 0; i < 250; i++)
						{
							var type = GetRandomRewardType();
							var payout = avaliable.GetPercentage(0.01M);
							var time = start.AddMilliseconds(_random.Next(maxMilliSeconds));
							if (payout < 0.00000001M)
							{
								break;
							}
							if (_rewardSchedule.ContainsKey(time))
							{
								i--;
								continue;
							}
							_rewardSchedule.Add(time, new RewardItem
							{
								Currency = balance.CurrencyId,
								Amount = payout,
								RewardType = type,
								Percent = 0.01M,
								Message =
									string.Format("Your {0} triggered a 0.01% {1} reward!, You received {2} {1}", type.ToString(), balance.Symbol,
										payout.ToString("F8"))
							});
							avaliable -= payout;
						}

						// 0.1%
						for (int i = 0; i < 25; i++)
						{
							var type = GetRandomRewardType();
							var payout = avaliable.GetPercentage(0.1M);
							var time = start.AddMilliseconds(_random.Next(maxMilliSeconds));
							if (payout < 0.00000001M)
							{
								break;
							}
							if (_rewardSchedule.ContainsKey(time))
							{
								i--;
								continue;
							}
							_rewardSchedule.Add(time, new RewardItem
							{
								Currency = balance.CurrencyId,
								Amount = payout,
								RewardType = type,
								Percent = 0.1M,
								Message =
									string.Format("Your {0} triggered a 0.1% {1} reward!, You received {2} {1}", type.ToString(), balance.Symbol,
										payout.ToString("F8"))
							});
							avaliable -= payout;
						}

						// 1%
						for (int i = 0; i < 3; i++)
						{
							var type = GetRandomRewardType();
							var payout = avaliable.GetPercentage(1M);
							var time = start.AddMilliseconds(_random.Next(maxMilliSeconds));
							if (payout < 0.00000001M)
							{
								break;
							}
							if (_rewardSchedule.ContainsKey(time))
							{
								i--;
								continue;
							}
							_rewardSchedule.Add(time, new RewardItem
							{
								Currency = balance.CurrencyId,
								Amount = payout,
								RewardType = type,
								Percent = 1,
								Message =
									string.Format("Your {0} triggered a 1% {1} reward!, You received {2} {1}", type.ToString(), balance.Symbol,
										payout.ToString("F8"))
							});
							avaliable -= payout;
						}

						// 5%
						for (int i = 0; i < 1; i++)
						{
							var type = GetRandomRewardType();
							var payout = avaliable.GetPercentage(5M);
							var time = start.AddMilliseconds(_random.Next(maxMilliSeconds));
							if (payout < 0.00000001M)
							{
								break;
							}
							if (_rewardSchedule.ContainsKey(time))
							{
								i--;
								continue;
							}
							_rewardSchedule.Add(time, new RewardItem
							{
								Currency = balance.CurrencyId,
								Amount = payout,
								RewardType = type,
								Percent = 5,
								Message =
									string.Format("Your {0} triggered a 5% {1} reward!, You received {2} {1}", type.ToString(), balance.Symbol, payout.ToString("F8"))
							});
							avaliable -= payout;
						}

						//// 10%
						//if (balance.CurrencyId == randomCurrencyId)
						//{
						//	var type = GetRandomRewardType();
						//	var payout = avaliable.GetPercentage(10);
						//	var time = start.AddMilliseconds(_random.Next(maxMilliSeconds));
						//	if (!_rewardSchedule.ContainsKey(time))
						//	{
						//		_rewardSchedule.Add(time, new RewardItem
						//		{
						//			Currency = balance.CurrencyId,
						//			Amount = payout,
						//			RewardType = type,
						//			Percent = 10,
						//			Message =
						//				string.Format("Your {0} triggered todays 10% mega reward!, You received {2} {1}", type.ToString(),
						//					balance.Symbol, payout.ToString("F8"))
						//		});
						//	}
						//	avaliable -= payout;
						//}
					}
					Log.Message(LogLevel.Info, "[GenerateSchedule] - Generation complete, TotalRewards: {0}", _rewardSchedule.Count());
				}
			}
			catch (Exception ex)
			{
				Log.Exception("[GenerateSchedule] - An exception occured generating schedule.", ex);
			}
		}

		/// <summary>
		/// Rewards the user.
		/// </summary>
		/// <param name="rewardItem">The reward item.</param>
		private async Task RewardUser(RewardItem rewardItem)
		{
			try
			{
				var rewardUser = await GetRewardUser(rewardItem.RewardType);
				if (rewardUser == Guid.Empty)
				{
					Log.Message(LogLevel.Info, "[RewardUser] - No new reward user fround for reward, Reward: {0}.",
						rewardItem.RewardType);
					return;
				}

				Log.Message(LogLevel.Info,
					"[RewardUser] - Sending user reward transfer, User: {0}, Reward: {1}, CurrencyId: {2}, Amount: {3}", rewardUser,
					rewardItem.RewardType, rewardItem.Currency, rewardItem.Amount);
				using (var tradeService = new TradeProcessorClient())
				using (var context = ExchangeDataContextFactory.CreateContext())
				{
					var response = await tradeService.SubmitTransferAsync(new SubmitTransferRequest
					{
						UserTo = rewardUser,
						UserId = Constant.SYSTEM_USER_REWARD,
						TransferType = TransferType.Reward,
						CurrencyId = rewardItem.Currency,
						Amount = rewardItem.Amount,
						NotificationTitle = "Coin Reward!",
						NotificationMessage = rewardItem.Message
					});

					if (!string.IsNullOrEmpty(response.Error))
					{
						Log.Message(LogLevel.Error, "[RewardUser] - TradeProcessorClient faled to process transfer, Error: {0}",
							response.Error);
						return;
					}

					context.Reward.Add(new Reward
					{
						UserId = rewardUser,
						Amount = rewardItem.Amount,
						CurrencyId = rewardItem.Currency,
						RewardType = rewardItem.RewardType.ToString(),
						Message = rewardItem.Message,
						TimeStamp = DateTime.UtcNow,
						Percent = rewardItem.Percent
					});
					await context.SaveChangesAsync();
					Log.Message(LogLevel.Info, "[RewardUser] - Sending user reward transfer complete.");
				}
			}
			catch (Exception ex)
			{
				Log.Exception("[RewardUser] - An exception occured paying user reward", ex);
			}
		}

		/// <summary>
		/// Gets the reward user.
		/// </summary>
		/// <param name="rewardType">Type of the reward.</param>
		private async Task<Guid> GetRewardUser(RewardType rewardType)
		{
			try
			{
				Log.Message(LogLevel.Debug, "[GetRewardUser] - Fetching reward user, RewardType: {0}", rewardType);
				using (var context = ExchangeDataContextFactory.CreateContext())
				{
					var result = await context.Database.SqlQuery<RewardResult>("GetRewardUser @p0", rewardType.ToString()).ToListAsync();
					if (result.IsNullOrEmpty())
					{
						Log.Message(LogLevel.Debug, "[GetRewardUser] - No user found for reward.");
						return Guid.Empty;
					}

					var item = result.FirstOrDefault();
					if (_lastValues[rewardType].LastItemId == item.LastItemId || _lastValues[rewardType].UserId == item.UserId)
					{
						Log.Message(LogLevel.Debug, "[GetRewardUser] - User already awarded for this prize, LastItemId: {0}", item.LastItemId);
						return Guid.Empty;
					}

					_lastValues[rewardType] = item;
					Log.Message(LogLevel.Debug, "[GetRewardUser] - User found for reward, UserId: {0}, ItemId: {1}", item.UserId, item.LastItemId);
					return item.UserId;
				}
			}
			catch (Exception ex)
			{
				Log.Exception("[GetRewardUser] - An exception occured getting reward user, RewardType: {0}", ex, rewardType);
			}
			return Guid.Empty;
		}

		/// <summary>
		/// Gets the last reward items.
		/// </summary>
		private async Task GetLastRewardItems()
		{
			try
			{
				Log.Message(LogLevel.Debug, "[GetLastRewardItems] - Getting last rewarded item ids...");
				using (var context = ExchangeDataContextFactory.CreateContext())
				{
					_lastValues.Clear();
					foreach (var rewardType in Enum.GetValues(typeof(RewardType)).OfType<RewardType>())
					{
						Log.Message(LogLevel.Debug, "[GetLastRewardItems] - Getting last rewarded item id for {0}", rewardType);
						var result = await context.Database.SqlQuery<RewardResult>("GetRewardUser @p0", rewardType.ToString()).ToListAsync();
						if (result.IsNullOrEmpty())
						{
							Log.Message(LogLevel.Debug, "[GetLastRewardItems] - No last item found.");
							_lastValues.Add(rewardType, new RewardResult());
							continue;
						}

						var lastId = result.FirstOrDefault();
						Log.Message(LogLevel.Debug, "[GetLastRewardItems] - Last item found, Id: {0}", lastId);
						_lastValues.Add(rewardType, lastId);
					}
				}
				Log.Message(LogLevel.Debug, "[GetLastRewardItems] - Getting last rewarded item ids complete.");
			}
			catch (Exception ex)
			{
				Log.Exception("[GetLastRewardItems] - An exception occured getting the last rewarded item ids.", ex);
			}
		}

		/// <summary>
		/// Gets a random RewardType from the weighted list
		/// </summary>
		/// <returns></returns>
		private RewardType GetRandomRewardType()
		{
			var random = _random.Next(_rewardTypeWeightedList.Count);
			return (RewardType)_rewardTypeWeightedList.ElementAt(random);
		}

		/// <summary>
		/// Creates the reward type weighted list.
		/// </summary>
		private void CreateRewardTypeWeightedList()
		{
			_rewardTypeWeightedList.Clear();
			for (int i = 0; i < 8; i++)
			{
				_rewardTypeWeightedList.Add(RewardType.Trade);
				//_rewardTypeWeightedList.Add(RewardType.Block);
			}
			for (int i = 0; i < 4; i++)
			{
				//_rewardTypeWeightedList.Add(RewardType.LastShare);
				_rewardTypeWeightedList.Add(RewardType.Tip);
			}
			_rewardTypeWeightedList.Add(RewardType.Chat);
		}

		private async Task<bool> IsRegenerationRequired()
		{
			using (var context = ExchangeDataContextFactory.CreateContext())
			{
				var rewardBalanceCount = await context.Balance
						.Where(b => b.UserId == Constant.SYSTEM_USER_REWARD && b.Currency.RewardsExpires > DateTime.UtcNow && b.Total > 0)
						.CountNoLockAsync();
				return _rewardCurrencyCount != rewardBalanceCount;
			}
		}

		#endregion
	}
}