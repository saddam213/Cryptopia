using System;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Cryptopia.Base.Logging;
using System.Collections.Generic;
using Cryptopia.Entity;
using Cryptopia.Common.DataContext;
using Cryptopia.Data.DataContext;
using System.Data.Entity;
using Cryptopia.Enums;
using Cryptopia.WalletAPI.Base;
using Cryptopia.Base.Extensions;
using Cryptopia.Common.MineshaftNotification;
using Cryptopia.PoolService.TradeService;
using Cryptopia.Base;
using Cryptopia.Infrastructure.Common.DataContext;

namespace Cryptopia.PoolService.Implementation
{
	public class PoolTracker
	{
		#region Fields

		private bool _isEnabled = true;
		private CancellationToken _cancelToken;
		private static readonly Log Log = LoggingManager.GetLog(typeof(PoolTracker));

		private bool _isProcessorRunning;
		private double _hashRateCalculationPeriod = 300.0; // 5 minutes
		private int _statisticPollPeriod = 10; // 10 seconds
		private int _payoutPollPeriod = 1; // 15 minutes
		private int _sitePayoutPollPeriod = 180; // 3hours
		private int _profitabilityPollPeriod = 15;
		private bool _profitSwitchEnabled;
		private bool _processorEnabled;
		private decimal _profitDepthBTC = 0.10000000m;
		private decimal _profitDepthLTC = 10.00000000m;
		private DateTime _nextUserPayoutPoll = DateTime.UtcNow;
		private DateTime _nextSitePayoutPoll = DateTime.UtcNow;
		private DateTime _nextProfitabilityPoll = DateTime.UtcNow;

		#endregion

		#region Constructor

		public PoolTracker(CancellationToken cancelToken)
		{
			_cancelToken = cancelToken;
			PoolDataContextFactory = new PoolDataContextFactory();
			ExchangeDataContextFactory = new ExchangeDataContextFactory();
		}

		#endregion

		#region Properties

		public IPoolDataContextFactory PoolDataContextFactory { get; set; }
		public IExchangeDataContextFactory ExchangeDataContextFactory { get; set; }

		public bool Running
		{
			get { return _isProcessorRunning; }
		}

		#endregion

		#region Public Members

		public void Start()
		{
			_isProcessorRunning = true;
			_nextUserPayoutPoll = DateTime.UtcNow.AddMinutes(_payoutPollPeriod);
			_nextSitePayoutPoll = DateTime.UtcNow;
			Task.Factory.StartNew(async () => await ProcessPools().ConfigureAwait(false), _cancelToken, TaskCreationOptions.LongRunning, TaskScheduler.Default).ConfigureAwait(false);
		}

		public void Stop()
		{
			Log.Message(LogLevel.Info, "[Stop] - Stopping pool poll loop.");
			_isEnabled = false;
		}



		private async Task ProcessPools()
		{
			while (_isEnabled)
			{
				try
				{
					var statStart = DateTime.UtcNow;
					await LoadSettings();

					if (_processorEnabled)
					{
						await RunStatistics().ConfigureAwait(false);

						//Payouts
						if (DateTime.UtcNow > _nextUserPayoutPoll)
						{
							await RunPayouts().ConfigureAwait(false);
							_nextUserPayoutPoll = DateTime.UtcNow.AddMinutes(_payoutPollPeriod);
						}

						// Site Payouts
						if (DateTime.UtcNow > _nextSitePayoutPoll)
						{
							await RunSitePayouts();
							_nextSitePayoutPoll = DateTime.UtcNow.AddMinutes(_sitePayoutPollPeriod);
						}

						// Profitability
						if (DateTime.UtcNow > _nextProfitabilityPoll)
						{
							await RunProfitability().ConfigureAwait(false);
							_nextProfitabilityPoll = DateTime.UtcNow.AddMinutes(_profitabilityPollPeriod);
						}
					}

					if (!_isEnabled)
						break;

					var statsEnd = (DateTime.UtcNow - statStart);
					var delay = _statisticPollPeriod - statsEnd.TotalSeconds;
					if (delay > 0)
					{
						Log.Message(LogLevel.Info, "[ProcessPools] - Waiting {0} seconds...", delay);
						await Task.Delay(TimeSpan.FromSeconds(delay), _cancelToken).ConfigureAwait(false);
					}
				}
				catch (TaskCanceledException)
				{
					Log.Message(LogLevel.Info, "[ProcessPools] - Processing canceled");
					break;
				}
			}

			Log.Message(LogLevel.Info, "[ProcessPools] - Processing stopped");
			_isProcessorRunning = false;
		}


		#endregion

		#region Private Members

		private async Task LoadSettings()
		{
			using (var context = PoolDataContextFactory.CreateContext())
			{
				var settings = await context.Settings.FirstOrDefaultNoLockAsync();
				if (settings == null)
					throw new Exception("No settings found.");

				_hashRateCalculationPeriod = settings.HashRateCalculationPeriod; // 5 minutes
				_statisticPollPeriod = settings.StatisticsPollPeriod; // 10 seconds
				_payoutPollPeriod = settings.PayoutPollPeriod; // 15 minutes
				_sitePayoutPollPeriod = settings.SitePayoutPollPeriod; // 3hours
				_profitabilityPollPeriod = settings.ProfitabilityPollPeriod;
				_profitSwitchEnabled = settings.ProfitSwitchEnabled;
				_processorEnabled = settings.ProcessorEnabled;
				_profitDepthBTC = settings.ProfitSwitchDepthBTC;
				_profitDepthLTC = settings.ProfitSwitchDepthLTC;
			}
		}



		private static string _queryRoundShares =
			@"SELECT 
					UserId,
					IsValidShare,
					IsValidBlock,
					SUM(Difficulty) AS Shares
				FROM Shares_{0}
				WHERE Id > @p0
				GROUP BY UserId,
				IsValidShare,
				IsValidBlock";

		private static string _queryBlockShares =
			@"SELECT 
					UserId,
					IsValidShare,
					IsValidBlock,
					SUM(Difficulty) AS Shares
				FROM Shares_{0}
				WHERE IsProcessed = 0 AND Id BETWEEN @p0 AND @p1
				GROUP BY UserId,
				IsValidShare,
				IsValidBlock";

		private static string _queryStatisticShares =
			@"SELECT 
					WorkerName,
					SUM(Difficulty) AS Shares
				FROM Shares_{0}
				WHERE Timestamp > @p0 AND (IsValidShare = 1 OR IsValidBlock = 1)
				GROUP BY WorkerName";

		private async Task RunStatistics()
		{
			Log.Message(LogLevel.Info, "[RunStatistics] - RunStatistics started...");
			using (var context = PoolDataContextFactory.CreateContext())
			{
				context.Database.CommandTimeout = 240;
				try
				{
					var dataNotifications = new List<IMineshaftNotification>();
					Log.Message(LogLevel.Debug, $"[RunStatistics] - Fetching pool data...");
					// Get the pools
					var pools = await context.Pool
							.Include(p => p.Statistics)
							.Where(p => p.IsEnabled && (p.Status == PoolStatus.OK || p.Status == PoolStatus.Expired || p.Status == PoolStatus.Expiring))
							.ToListNoLockAsync();
					Log.Message(LogLevel.Debug, $"[RunStatistics] - Fetching pool data complete.");


					foreach (var pool in pools)
					{
						try
						{
							if (!_isEnabled)
								return;

							Log.Message(LogLevel.Info, $"[RunStatistics] - Processing statistics for {pool.Symbol}...");
							var poolHashrate = 0.0;
							var activeUsers = new HashSet<Guid>();
							var statisticShareTime = DateTime.UtcNow.AddSeconds(-_hashRateCalculationPeriod);
							var roundShares = await context.Database.SqlQuery<RoundData>(string.Format(_queryRoundShares, pool.TablePrefix), pool.Statistics.LastPayoutShareId).ToListNoLockAsync();
							var statisticShares = await context.Database.SqlQuery<StatisticData>(string.Format(_queryStatisticShares, pool.TablePrefix), statisticShareTime).ToListNoLockAsync();

							var poolMinerIds = roundShares.Select(x => x.UserId).Distinct().ToList();
							var poolMiners = await context.Worker
								.Include(x => x.User.Statistics)
								.Where(x => x.AlgoType == pool.AlgoType && poolMinerIds.Contains(x.UserId))
								.ToListNoLockAsync();


							var invalidRoundShares = roundShares.Where(x => !x.IsValidShare).ToList();
							var validRoundShares = roundShares.Where(x => x.IsValidShare || x.IsValidBlock).ToList();


							foreach (var poolMiner in poolMiners.GroupBy(x => x.UserId))
							{
								if (!_isEnabled)
									return;

								activeUsers.Add(poolMiner.Key);
								var user = poolMiner.First().User;
								Log.Message(LogLevel.Debug, $"[RunStatistics] - Processing statistics for {user.MiningHandle}...");
								var poolWorkerStats = user.Statistics.FirstOrDefault(x => x.PoolId == pool.Id);
								if (poolWorkerStats == null)
								{
									poolWorkerStats = new PoolUserStatistics
									{
										PoolId = pool.Id,
										UserId = user.Id
									};
									user.Statistics.Add(poolWorkerStats);
								}

								poolWorkerStats.Hashrate = 0;
								poolWorkerStats.ValidShares = validRoundShares.Where(x => x.UserId == poolMiner.Key).Sum(x => x.Shares);
								poolWorkerStats.InvalidShares = invalidRoundShares.Where(x => x.UserId == poolMiner.Key).Sum(x => x.Shares);

								// If the miners workers are active on this pool, calculate the worker hashrate and notify user
								foreach (var worker in poolMiner.Where(x => x.TargetPool == pool.Symbol && (x.IsActive || x.Hashrate > 0)))
								{
									if (!_isEnabled)
										return;

									Log.Message(LogLevel.Debug, $"[RunStatistics] - Processing worker statistics for {worker.Name}...");
									var workerStatisticShares = statisticShares.Where(x => x.WorkerName.Equals(worker.Name, StringComparison.OrdinalIgnoreCase)).Sum(x => x.Shares);
									var workerHashrate = PoolExtensions.CalculateHashrate(workerStatisticShares, pool.AlgoType, _hashRateCalculationPeriod);
									worker.Hashrate = workerHashrate;
									poolWorkerStats.Hashrate += workerHashrate;
									poolHashrate += workerHashrate;

									// Add Worker notifications
									dataNotifications.Add(new WorkerNotification
									{
										PoolId = pool.Id,
										UserId = worker.UserId,
										Id = worker.Id,
										Name = worker.Name,
										MiningHandle = worker.User.MiningHandle,
										Hashrate = worker.Hashrate,
										Difficulty = worker.Difficulty
									});

									Log.Message(LogLevel.Debug, $"[RunStatistics] - Processing worker statistics for {worker.Name} complete.");
								}

								// Add User notifications
								dataNotifications.Add(new UserStatisticNotification
								{
									PoolId = pool.Id,
									MiningHandle = user.MiningHandle,
									Hashrate = poolWorkerStats.Hashrate,
									InvalidShares = poolWorkerStats.InvalidShares,
									ValidShares = poolWorkerStats.ValidShares,
									WorkerCount = poolMiner.Count(x => x.Hashrate > 0)
								});

								Log.Message(LogLevel.Debug, $"[RunStatistics] - Processing statistics for {user.MiningHandle} complete.");
							}

							// Reset the hash rate on any inactive users
							var inactiveUsers = await context.UserStatistics.Where(x => x.PoolId == pool.Id && x.Hashrate > 0 && !activeUsers.Contains(x.UserId)).ToListNoLockAsync();
							if (inactiveUsers.Any())
							{
								Log.Message(LogLevel.Debug, $"[RunStatistics] - Processing statistics for inactive users...");
								foreach (var inactiveUser in inactiveUsers)
								{
									inactiveUser.Hashrate = 0;

									// Add User notifications
									dataNotifications.Add(new UserStatisticNotification
									{
										PoolId = pool.Id,
										MiningHandle = inactiveUser.User.MiningHandle,
										Hashrate = inactiveUser.Hashrate,
										InvalidShares = inactiveUser.InvalidShares,
										ValidShares = inactiveUser.ValidShares,
										WorkerCount = 0
									});
								}
								Log.Message(LogLevel.Debug, $"[RunStatistics] - Processing statistics for inactive users complete.");
							}


							// Update pool stats
							pool.Statistics.ValidShares = validRoundShares.Sum(x => x.Shares);
							pool.Statistics.InvalidShares = invalidRoundShares.Sum(x => x.Shares);
							pool.Statistics.Hashrate = poolHashrate;
							pool.Statistics.NetworkHashrate = PoolExtensions.GetEstimatedNetworkHashrate(pool.AlgoType, pool.Statistics.NetworkDifficulty, pool.BlockTime);
							pool.Statistics.EstimatedShares = PoolExtensions.CalculateEstimatedShares(pool.Statistics.NetworkDifficulty, pool.AlgoType);
							pool.Statistics.BlockProgress = PoolExtensions.GetBlockProgress(pool.Statistics.EstimatedShares, pool.Statistics.ValidShares);
							pool.Statistics.EstimatedTime = PoolExtensions.GetEstimatedBlockTime(pool.Statistics.EstimatedShares, statisticShares.Sum(x => x.Shares), _hashRateCalculationPeriod);

							// Add pool status notification
							dataNotifications.Add(new StatisticNotification
							{
								PoolId = pool.Id,
								ValidShares = pool.Statistics.ValidShares,
								InvalidShares = pool.Statistics.InvalidShares,
								Hashrate = pool.Statistics.Hashrate,
								NetworkHashrate = pool.Statistics.NetworkHashrate,
								EstimatedShares = pool.Statistics.EstimatedShares,
								BlockProgress = pool.Statistics.BlockProgress,
								EstimatedTime = pool.Statistics.EstimatedTime,
								Connections = pool.Statistics.Connections,
								CurrentBlock = pool.Statistics.CurrentBlock,
								LastBlockTime = pool.Statistics.LastBlockTime,
								LastPoolBlock = pool.Statistics.LastPoolBlock,
								NetworkDifficulty = pool.Statistics.NetworkDifficulty,
								UserCount = pool.UserStatistics.Count(x => x.Hashrate > 0)
							});

							await context.SaveChangesAsync();
							await SendDataNotifications(dataNotifications);
							Log.Message(LogLevel.Info, $"[RunStatistics] - Processing statistics for pool complete.");


							// Check expiry status
							if (DateTime.UtcNow > pool.Expires && pool.Status != PoolStatus.Expired)
							{
								pool.Status = PoolStatus.Expired;
								var poolConnection = await context.Connection.Where(x => x.AlgoType == pool.AlgoType).FirstOrDefaultNoLockAsync();
								var expiredPoolWorkers = await context.Worker.Where(x => x.TargetPool == pool.Symbol).ToListNoLockAsync();
								foreach (var expiredPoolWorker in expiredPoolWorkers)
								{
									if (expiredPoolWorker.TargetPool == pool.Symbol)
										expiredPoolWorker.TargetPool = poolConnection.DefaultPool;
								}
								await context.SaveChangesAsync();
							}

							if (DateTime.UtcNow.AddDays(3) > pool.Expires && pool.Status == PoolStatus.OK)
							{
								pool.Status = PoolStatus.Expiring;
								pool.Statistics.Profitability = 0;
								await context.SaveChangesAsync();
							}
						}
						catch (Exception ex)
						{
							Log.Exception($"An error occurred processing {pool.Symbol} statistics.", ex);
						}
					}

					var unprocessedBlocks = await context.Blocks
						.Include(b => b.Pool)
						.Include(b => b.Payouts)
						.Where(x => !x.IsProcessed &&  x.Pool.Status == PoolStatus.OK)
						.OrderBy(x => x.Height)
						.ToListNoLockAsync();
					if (unprocessedBlocks.Any())
					{
						foreach (var block in unprocessedBlocks)
						{
							var pool = block.Pool;
							if (!block.IsProcessed)
							{
								// 1. Calculate user shares and insert user payouts
								var blockShareId = await context.Database.SqlQuery<int>($"SELECT Id FROM Shares_{pool.TablePrefix} WHERE BlockHash = @p0", block.Hash).FirstOrDefaultNoLockAsync();
								var sharesForBlock = await context.Database.SqlQuery<RoundData>(string.Format(_queryBlockShares, pool.TablePrefix), pool.Statistics.LastPayoutShareId, blockShareId).ToListNoLockAsync();
								var totalValidShares = sharesForBlock.Where(x => x.IsValidShare || x.IsValidBlock).Sum(x => x.Shares);

								if (block.Status != PoolBlockStatus.Orphan)
								{
									Log.Message(LogLevel.Info, $"[RunStatistics] - Processing new block for {pool.Name}, BlockHash: {block.Hash}...");

									if (block.Amount > 0 && totalValidShares > 0)
									{
										var amountPerShare = block.Amount / (decimal)totalValidShares;
										foreach (var userShares in sharesForBlock.Where(x => x.IsValidShare || x.IsValidBlock).GroupBy(x => x.UserId))
										{
											if (!userShares.Any())
												continue;

											var shares = userShares.Sum(x => x.Shares);
											var payout = new PoolUserPayout
											{
												PoolId = pool.Id,
												BlockId = block.Id,
												Status = PoolPayoutStatus.Unconfirmed,
												Shares = shares,
												Amount = Math.Round(amountPerShare * (decimal)shares, 8),
												UserId = userShares.Key
											};
											block.Payouts.Add(payout);
											Log.Message(LogLevel.Info, $"[RunStatistics] - Added new payout for user, UserId: {userShares.Key}, Shares: {payout.Shares:F8}, Amount: {payout.Amount}");
										}
									}

									await context.Database.ExecuteSqlCommandAsync($"UPDATE Shares_{pool.TablePrefix} SET IsProcessed = 1 WHERE Id BETWEEN @p0 AND @p1", pool.Statistics.LastPayoutShareId, blockShareId);
									//await context.Database.ExecuteSqlCommandAsync($"DELETE FROM Shares_{pool.TablePrefix} WHERE IsProcessed = 1 AND Timestamp < @p0", DateTime.UtcNow.AddDays(-7));
									pool.Statistics.LastPayoutShareId = blockShareId + 1;
									pool.Statistics.LastPoolBlock = block.Height;
									pool.Statistics.LastBlockTime = block.Timestamp;
								}

								block.IsProcessed = true;
								block.Shares = totalValidShares;
								block.EstimatedShares = PoolExtensions.CalculateEstimatedShares(block.Difficulty, pool.AlgoType);
								block.Luck = PoolExtensions.GetBlockProgress(block.EstimatedShares, totalValidShares);
								Log.Message(LogLevel.Info, $"[RunStatistics] - Processing new block for {pool.Name} complete.");
							}
						}
						await context.SaveChangesAsync();

						// Add block notifications
						foreach (var block in unprocessedBlocks)
						{
							foreach (var payout in block.Payouts)
							{
								dataNotifications.Add(new PayoutNotification
								{
									Id = payout.Id,
									PoolId = payout.PoolId,
									UserId = payout.UserId,
									Block = payout.Block.Height,
									Amount = payout.Amount,
									Status = payout.Status.ToString(),
									Shares = payout.Shares,
									TransferId = payout.TransferId,
									Timestamp = payout.Block.Timestamp
								});
							}
							dataNotifications.Add(new BlockNotification
							{
								PoolId = block.PoolId,
								Height = block.Height,
								Luck = block.Luck,
								Finder = block.User.MiningHandle,
								Amount = block.Amount,
								Difficulty = block.Difficulty,
								Confirmations = block.Confirmations,
								Status = block.Status.ToString(),
								Shares = block.Shares,
								Timestamp = block.Timestamp
							});
						}
						await SendDataNotifications(dataNotifications);
					}
				}
				catch (Exception ex)
				{
					Log.Exception("An error occurred processing pool statistics.", ex);
				}
			}
		}



		private async Task RunSitePayouts()
		{
			Log.Message(LogLevel.Info, $"[RunSitePayouts] - Processing site payouts...");
			var payouts = new List<SitePayoutInfo>();
			var poolsDueForPayout = new List<Entity.Pool>();
			using (var context = PoolDataContextFactory.CreateContext())
			{
				// Transfer funds to exchange wallet if required
				poolsDueForPayout = await context.Pool
					.Include(p => p.Statistics)
					.Where(x => x.IsEnabled && x.Status != PoolStatus.Maintenance)
					.ToListNoLockAsync();
				if (!poolsDueForPayout.Any())
				{
					Log.Message(LogLevel.Info, $"[RunSitePayouts] - No pools due for payout, skipping.");
					return;
				}
			}

			// Process pools
			foreach (var poolDueForPayout in poolsDueForPayout)
			{
				if (!_isEnabled)
					return;

				Log.Message(LogLevel.Info, $"[RunSitePayouts] - Processing site payment for {poolDueForPayout.Symbol}...");
				var amount = poolDueForPayout.Statistics.Balance - poolDueForPayout.WalletFee;
				if (amount < poolDueForPayout.WalletFee)
				{
					Log.Message(LogLevel.Info, $"[RunSitePayouts] - Site payment {amount:F8} {poolDueForPayout.Symbol} too small, skipping...");
					continue;
				}

				payouts.Add(new SitePayoutInfo
				{
					Amount = amount,
					CurrencyId = poolDueForPayout.CurrencyId,
					Symbol = poolDueForPayout.Symbol,
					Host = poolDueForPayout.WalletHost,
					Password = poolDueForPayout.WalletPass,
					Port = poolDueForPayout.WalletPort,
					UserName = poolDueForPayout.WalletUser
				});

				Log.Message(LogLevel.Info, $"[RunSitePayouts] - Processed site payment for {poolDueForPayout.Symbol}, Amount: {amount:F8}.");
			}

			if (!payouts.Any())
			{
				Log.Message(LogLevel.Info, $"[RunSitePayouts] - No payments required.");
				return;
			}

			// send the coins to exchange
			foreach (var payout in payouts)
			{
				if (!_isEnabled)
					return;

				Log.Message(LogLevel.Info, $"[RunSitePayouts] - Sending site payment for {payout.Symbol}...");
				if (payout.Amount <= 0)
				{
					Log.Message(LogLevel.Error, $"[RunSitePayouts] - Withdraw amount is 0");
					continue;
				}

				var exchangeAddress = await GetCurrencyExchangeAddress(payout.CurrencyId);
				if (string.IsNullOrEmpty(exchangeAddress))
				{
					Log.Message(LogLevel.Error, $"[RunSitePayouts] - Exchange address for currency {payout.Symbol} not found");
					continue;
				}

				try
				{
					var walletConnector = new WalletConnector(payout.Host, payout.Port, payout.UserName, payout.Password);
					var result = walletConnector.SendToAddress(exchangeAddress, payout.Amount);
					if (result == null || string.IsNullOrEmpty(result.Txid))
					{
						Log.Message(LogLevel.Error, $"[RunSitePayouts] - Failed to send funds to exchange wallet, Currency: {payout.Symbol}, Amount: {payout.Amount}.");
						continue;
					}
					Log.Message(LogLevel.Info, $"[RunSitePayouts] - Successfully sent funds to exchange wallet, Currency: {payout.Symbol}, Amount: {payout.Amount}, TxId: {result.Txid}");
				}
				catch (Exception ex)
				{
					Log.Exception("[RunSitePayouts] - An exception occurred sending funds to exchange wallet", ex);
				}

				
			}
			Log.Message(LogLevel.Info, $"[RunSitePayouts] - Processing site payouts complete.");
		}


		private async Task RunPayouts()
		{
			try
			{
				Log.Message(LogLevel.Info, $"[RunPayouts] - Processing payouts...");
				using (var context = PoolDataContextFactory.CreateContext())
				{
					var confirmedBlocks = await context.Blocks
						.Include(b => b.Pool)
						.Include(b => b.Payouts)
						.Where(b => b.IsProcessed && b.Status == PoolBlockStatus.Confirmed)
						.ToListNoLockAsync();

					if (!confirmedBlocks.Any())
					{
						Log.Message(LogLevel.Info, $"[RunPayouts] - No confirmed block found to payout.");
						return;
					}

					foreach (var poolBlocks in confirmedBlocks.GroupBy(x => x.PoolId))
					{
						if (!_isEnabled)
							return;

						var pool = poolBlocks.First().Pool;
						foreach (var block in poolBlocks)
						{
							if (!_isEnabled)
								return;

							block.Status = PoolBlockStatus.Pending;
							foreach (var payout in block.Payouts.Where(x => x.Status == PoolPayoutStatus.Unconfirmed))
							{
								payout.Status = PoolPayoutStatus.Pending;
							}
							Log.Message(LogLevel.Info, $"[RunPayouts] - Updated status for block {block.Id} and payments to {block.Status}");
						}

						// Save status before running payouts.
						await context.SaveChangesAsync();

						// Run payouts
						var pendingBlocks = poolBlocks.Where(x => x.PoolId == poolBlocks.Key && x.Status == PoolBlockStatus.Pending);
						if (pendingBlocks.Any())
						{
							Log.Message(LogLevel.Info, $"[RunPayouts] - Running payout for {pool.Name} pending blocks...");

							// 1. Check for fork
							var lastConfirmedBlock = pendingBlocks.OrderByDescending(x => x.Height).FirstOrDefault();
							if (lastConfirmedBlock != null)
							{
								Log.Message(LogLevel.Info, $"[RunPayouts] - Checking last confirmed block is on same fork as exchange wallet...");
								if (!pool.IsForkCheckDisabled && !await IsSameForkAsExchange(lastConfirmedBlock.Pool.CurrencyId, lastConfirmedBlock.Height, lastConfirmedBlock.Hash))
								{
									// Coin has forked, skip payouts
									pool.Status = PoolStatus.Maintenance;
									pool.StatusMessage = "Possible network fork, pool suspended.";
									await MovePoolWorkers(context, pool);
									await context.SaveChangesAsync();
									Log.Message(LogLevel.Error, $"[RunPayouts] - Blockhash for block {lastConfirmedBlock.Height} does not match BlockHash in exchange wallet.");
									continue;
								}
								Log.Message(LogLevel.Info, $"[RunPayouts] - Confirmed fork is correct.");
							}

							// Payout any blocks in pending
							var payouts = pendingBlocks.SelectMany(x => x.Payouts).Where(x => x.Status == PoolPayoutStatus.Pending);
							if (payouts.Any())
							{
								Log.Message(LogLevel.Info, $"[RunPayouts] - Transferring funds to users.");
								var paymentData = payouts.Select(x => new TradeService.PoolPayment
								{
									PaymentId = x.Id,
									Amount = x.Amount,
									UserId = x.UserId
								}).ToList();

								foreach (var paymentGroup in paymentData.Batch(50))
								{
									Log.Message(LogLevel.Debug, $"[RunPayouts] - Transferring batch of 50 payments to trade engine.");
									var transferResponse = await TransferUserFunds(pool.CurrencyId, paymentGroup.ToList());
									if (transferResponse.Any())
									{
										var ids = transferResponse.Select(x => x.PaymentId).ToList();
										var payoutsToUpdate = await context.UserPayout.Where(x => ids.Contains(x.Id)).ToListNoLockAsync();
										foreach (var payoutToUpdate in payoutsToUpdate)
										{
											var response = transferResponse.FirstOrDefault(x => x.PaymentId == payoutToUpdate.Id);
											if (response == null)
												continue;

											Log.Message(LogLevel.Debug, $"[RunPayouts] - Transfered funds to user, UserId: {payoutToUpdate.UserId}, Amount: {payoutToUpdate.Amount}.");
											payoutToUpdate.TransferId = response.TransferId;
											payoutToUpdate.Status = PoolPayoutStatus.Complete;
											payoutToUpdate.Block.Status = PoolBlockStatus.Complete;
										}

										await context.SaveChangesAsync();
									}
								}

								
							}
							Log.Message(LogLevel.Info, $"[RunPayouts] - Running payout for {pool.Name} pending blocks complete.");
						}
					}
				}
			}
			catch (Exception ex)
			{
				Log.Exception($"[RunPayouts] - An exception occurred processing payouts.", ex);
			}
		}

		public async Task RunProfitability()
		{
			try
			{
				Log.Message(LogLevel.Info, "[UpdatePoolProfitability] - RunProfitability started...");
				var profitData = new Dictionary<int, decimal>();
				var priceForDepth = new Dictionary<int, decimal>();
				var poolDataList = new List<PoolProfitabilityData>();
				using (var context = PoolDataContextFactory.CreateContext())
				{
					poolDataList = await context.Pool
						.Where(x => x.IsEnabled && (x.Status == PoolStatus.OK || x.Status == PoolStatus.Expiring))
						.Select(x => new PoolProfitabilityData
						{
							PoolId = x.Id,
							Symbol = x.Symbol,
							CurrencyId = x.CurrencyId,
							Difficulty = x.Statistics.NetworkDifficulty,
							BlockReward = x.Statistics.BlockReward,
							BlockTime = x.BlockTime,
							AlgoType = x.AlgoType
						}).ToListNoLockAsync();
				}

				// Get trade price data
				Log.Message(LogLevel.Debug, "[UpdatePoolProfitability] - Calculating trade depth/price data...");
				using (var context = ExchangeDataContextFactory.CreateContext())
				{
					var currencyIds = poolDataList.Select(x => x.CurrencyId).Distinct().ToList();
					var ltcTradePair = await context.TradePair.Where(x => x.CurrencyId1 == Constant.LITECOIN_ID && x.CurrencyId2 == Constant.BITCOIN_ID).FirstOrDefaultNoLockAsync();

					var tradepairs = await context.TradePair.Where(x => currencyIds.Contains(x.CurrencyId1) && (x.CurrencyId2 == Constant.LITECOIN_ID || x.CurrencyId2 == Constant.BITCOIN_ID)).ToListNoLockAsync();
					foreach (var currencyId in poolDataList.Select(x => x.CurrencyId).Distinct())
					{

						var isLitecoin = false;
						var tradepairId = 0;
						var btcPair = tradepairs.FirstOrDefault(x => x.CurrencyId1 == currencyId && x.CurrencyId2 == Constant.BITCOIN_ID);
						if (btcPair == null)
						{
							var ltcPair = tradepairs.FirstOrDefault(x => x.CurrencyId1 == currencyId && x.CurrencyId2 == Constant.LITECOIN_ID);
							if (ltcPair == null)
								continue;

							isLitecoin = true;
							tradepairId = ltcPair.Id;
						}
						else
						{
							tradepairId = btcPair.Id;
						}

						var buyDepth = 0m;
						var prices = new List<decimal>();
						var buyDepthLimit = isLitecoin ? _profitDepthLTC : _profitDepthBTC;
						var trades = await context.Trade
							.Where(x => x.TradePairId == tradepairId && x.Type == TradeHistoryType.Buy && (x.Status == TradeStatus.Partial || x.Status == TradeStatus.Pending))
							.Select(x => new
							{
								x.Rate,
								x.Remaining
							}).ToListNoLockAsync();

						foreach (var trade in trades.OrderByDescending(x => x.Rate))
						{
							buyDepth += trade.Remaining * trade.Rate;
							prices.Add(trade.Rate);
							if (buyDepth >= buyDepthLimit)
							{
								var price = isLitecoin
									? prices.Average() * ltcTradePair.LastTrade
									: prices.Average();
								if (!priceForDepth.ContainsKey(currencyId))
								{
									priceForDepth.Add(currencyId, price);
									break;
								}
							}
						}
					}
				}
				Log.Message(LogLevel.Debug, "[UpdatePoolProfitability] - Calculating trade depth/price data complete.");


				// Sort out price per mega-hash
				Log.Message(LogLevel.Debug, "[UpdatePoolProfitability] - Calculating price per MS/s...");
				foreach (var poolData in poolDataList)
				{
					if (!priceForDepth.ContainsKey(poolData.CurrencyId))
					{
						if (!profitData.ContainsKey(poolData.PoolId))
							profitData.Add(poolData.PoolId, 0m);
						continue;
					}

					var btcPerMhPerHour = 0m;
					var btcPrice = priceForDepth[poolData.CurrencyId];
					var estimatedNewtworkMegahash = 0d;
					var estimatedNewtworkHashrate = 0d;
					var estimatedCoinsPerHourPerMhs = 0m;
					var estimatedCoinsPerHour = 0m;
					if (poolData.Difficulty > 0 && btcPrice > 0 && poolData.BlockTime > 0)
					{
						//var blocksPerMhPerHour = 86400.0 / ((poolData.Difficulty * Math.Pow(2.0, 32.0)) / (1.0 * 1000.0 * 1000.0));
						//var coinsPerMhPerHour = poolData.BlockReward * (decimal)blocksPerMhPerHour;
						//btcPerMhPerHour = coinsPerMhPerHour * btcPrice;

						estimatedNewtworkHashrate = PoolExtensions.GetEstimatedNetworkHashrate(poolData.AlgoType, poolData.Difficulty, poolData.BlockTime);
						if (estimatedNewtworkHashrate > 0)
						{
							estimatedNewtworkMegahash = estimatedNewtworkHashrate / 1000.0 / 1000.0;
							estimatedCoinsPerHour = poolData.BlockReward * (3600.0m / poolData.BlockTime);
							if (estimatedNewtworkMegahash > 0)
							{
								estimatedCoinsPerHourPerMhs = estimatedCoinsPerHour / (decimal)estimatedNewtworkMegahash;
								btcPerMhPerHour = estimatedCoinsPerHourPerMhs * btcPrice;
							}
						}
						Log.Message(LogLevel.Debug, $"[UpdatePoolProfitability] - Pool: {poolData.Symbol}, Price: {btcPrice}, Hashrate: {estimatedNewtworkMegahash} Mh/s, CoinsPerHour: {estimatedCoinsPerHour}, CoinsPerHourPerMhs: {estimatedCoinsPerHourPerMhs}, BTCPerMhPerHour: {btcPerMhPerHour}, BTCPerMhPerDay: {btcPerMhPerHour * 24}");
					}

					if (!profitData.ContainsKey(poolData.PoolId))
						profitData.Add(poolData.PoolId, Math.Max(btcPerMhPerHour, 0));
				}
				Log.Message(LogLevel.Debug, "[UpdatePoolProfitability] - Calculating price per MS/s complete.");


				// Save the data
				using (var context = PoolDataContextFactory.CreateContext())
				{
					Log.Message(LogLevel.Debug, "[UpdatePoolProfitability] - Saving profitability data to stats table...");
					var poolIds = profitData.Keys.ToList();
					var pools = await context.Statistics.Where(x => poolIds.Contains(x.PoolId)).ToListNoLockAsync();
					foreach (var pool in pools)
					{
						pool.Profitability = profitData[pool.PoolId];
					}
					await context.SaveChangesAsync();
					Log.Message(LogLevel.Debug, "[UpdatePoolProfitability] - Saving profitability data to stats table complete.");

					if (_profitSwitchEnabled)
					{
						Log.Message(LogLevel.Debug, "[UpdatePoolProfitability] - Moving AutoSwitch workers to most profitable pool..");
						var profitWorkers = await context.Worker.Where(x => x.IsEnabled && x.IsAutoSwitch).ToListNoLockAsync();
						var profitPools = await context.Pool
							.Where(x => x.IsEnabled && (x.Status == PoolStatus.OK || x.Status == PoolStatus.Expiring) && x.Statistics.Profitability > 0)
							.GroupBy(x => x.AlgoType)
							.Select(x => x.OrderByDescending(c => c.Statistics.Profitability).FirstOrDefault())
							.ToListNoLockAsync();
						foreach (var workerGroup in profitWorkers.GroupBy(x => x.AlgoType))
						{
							var profitPool = profitPools.FirstOrDefault(x => x.AlgoType == workerGroup.Key);
							if (profitPool != null)
							{
								foreach (var worker in workerGroup)
								{
									worker.TargetPool = profitPool.Symbol;
								}
							}
						}
						await context.SaveChangesAsync();
						Log.Message(LogLevel.Debug, "[UpdatePoolProfitability] - Moving AutoSwitch workers to most profitable pool complete.");
					}
				}
				Log.Message(LogLevel.Info, "[UpdatePoolProfitability] - RunProfitability complete.");
			}
			catch (Exception ex)
			{
				Log.Exception("[UpdatePoolProfitability] - An exception occurred sending notifications", ex);
			}
		}

		private async static Task MovePoolWorkers(IPoolDataContext context, Pool pool)
		{
			var connection = await context.Connection.FirstOrDefaultNoLockAsync(x => x.AlgoType == pool.AlgoType);
			var backupPool = await context.Pool
							.Where(x => x.AlgoType == pool.AlgoType && x.Status == Enums.PoolStatus.OK && x.Id != pool.Id)
							.OrderByDescending(x => x.Statistics.Profitability)
							.FirstOrDefaultNoLockAsync();
			var workers = await context.Worker
				.Where(x => x.AlgoType == pool.AlgoType && x.TargetPool == pool.Symbol)
				.ToListNoLockAsync();
			foreach (var worker in workers)
			{
				// If the default pool is not this pool set as target
				if (connection.DefaultPool != pool.Symbol)
				{
					worker.TargetPool = connection.DefaultPool;
					continue;
				}

				// If everything is down use fist working pool
				if (backupPool != null)
				{
					worker.TargetPool = backupPool.Symbol;
					continue;
				}
				// if we are here the whole mineshaft is obviously down so do nothing
			}
		}

		private async static Task<bool> IsSameForkAsExchange(int currencyId, int blockHeight, string blockhash)
		{
			try
			{
				using (var service = new DepositService.WalletInboundClient())
				{
					var result = await service.GetBlockAsync(new DepositService.GetWalletBlockRequest
					{
						CurrencyId = currencyId,
						BlockHeight = blockHeight
					});

					if (result.BlockData != null)
						return result.BlockData.hash == blockhash;

					return false;
				}
			}
			catch (Exception ex)
			{
				Log.Exception("[IsSameForkAsExchange] - An exception occurred checking fork status", ex);
				return true;
			}
		}

		private async static Task<List<string>> GenerateMineshaftAddress(int currencyId)
		{
			try
			{
				using (var service = new DepositService.WalletInboundClient())
				{
					return await service.CreateAddressAsync(currencyId, Constant.SYSTEM_USER_MINESHAFT);
				}
			}
			catch (Exception ex)
			{
				Log.Exception("[GenerateMineshaftAddress] - An exception occurred generating mineshaft address", ex);
				return new List<string>();
			}
		}

		private async Task<string> GetCurrencyExchangeAddress(int currencyId)
		{
			try
			{
				using (var context = ExchangeDataContextFactory.CreateContext())
				{
					var address = await context.Address.FirstOrDefaultNoLockAsync(x => x.CurrencyId == currencyId && x.UserId == Constant.SYSTEM_USER_MINESHAFT);
					if (address == null)
					{
						var newAddress = await GenerateMineshaftAddress(currencyId);
						if (newAddress == null || newAddress.Count < 2)
							return null;

						address = new Entity.Address
						{
							AddressHash = newAddress[0],
							PrivateKey = newAddress[1],
							UserId = Constant.SYSTEM_USER_MINESHAFT,
							CurrencyId = currencyId
						};
						context.Address.Add(address);
						await context.SaveChangesAsync();
					}
					return address.AddressHash;
				}
			}
			catch (Exception ex)
			{
				Log.Exception("[GetCurrencyExchangeAddress] - An exception occurred sending funds to exchange wallet", ex);
				return null;
			}
		}

		private async static Task<List<TradeService.PoolPaymentResult>> TransferUserFunds(int currencyId, List<TradeService.PoolPayment> payments)
		{
			try
			{
				using (var service = new TradeService.TradeProcessorClient())
				{
					var result = await service.SubmitPoolPaymentAsync(new TradeService.SubmitPoolPaymentRequest
					{
						CurrencyId = currencyId,
						Payments = payments
					});
					if (string.IsNullOrEmpty(result.Error))
					{
						await SendNotifications(result.Notifications);
						return result.Payments;
					}
					return new List<TradeService.PoolPaymentResult>();
				}
			}
			catch (Exception ex)
			{
				Log.Exception("[TransferUserFunds] - An exception occurred transferring funds to exchange account", ex);
				return new List<TradeService.PoolPaymentResult>();
			}
		}

		private static Task SendNotifications(List<TradeNotification> notifications)
		{
			try
			{
				if (!notifications.Any())
					return Task.FromResult(0);

				//using (var notifier = new PoolNotifier())
				//{
				//	await notifier.SendNotifications(notifications);
				//	notifications.Clear();
				//}
			}
			catch (Exception ex)
			{
				Log.Exception("[SendNotifications] - An exception occurred sending notifications", ex);
			}

			return Task.FromResult(0);
		}

		private async static Task SendDataNotifications(List<IMineshaftNotification> dataNotifications)
		{
			try
			{
				if (!dataNotifications.Any())
					return;

				using (var notifier = new PoolNotifier())
				{
					await notifier.SendDataNotifications(dataNotifications);
					dataNotifications.Clear();
				}
			}
			catch (Exception ex)
			{
				Log.Exception("[SendDataNotifications] - An exception occurred sending notifications", ex);
			}
		}

		#endregion
	}

	public class RoundData
	{
		public Guid UserId { get; set; }
		public bool IsValidShare { get; set; }
		public bool IsValidBlock { get; set; }
		public double Shares { get; set; }
	}

	public class StatisticData
	{
		public string WorkerName { get; set; }
		public double Shares { get; set; }
	}

	public class PoolProfitabilityData
	{
		public AlgoType AlgoType { get; internal set; }
		public decimal BlockReward { get; internal set; }
		public int BlockTime { get; internal set; }
		public int CurrencyId { get; internal set; }
		public double Difficulty { get; internal set; }
		public int PoolId { get; internal set; }
		public string Symbol { get; internal set; }
	}

	public class SitePayoutInfo
	{
		public int Port { get; set; }
		public string Host { get; set; }
		public string UserName { get; set; }
		public string Password { get; set; }
		public decimal Amount { get; set; }
		public string Symbol { get; internal set; }
		public int CurrencyId { get; internal set; }
	}
}