using System;
using System.Linq;
using System.Threading.Tasks;
using Cryptopia.API.Logging.Base;
using Cryptopia.Logging;
using Cryptopia.WalletAPI.DataObjects;
using System.Collections.Generic;
using Cryptopia.Common.DataAccess;
using Cryptopia.API.Objects;
using Cryptopia.API.DataAccess;
using Cryptopia.WalletAPI.Base;
using Cryptopia.Common;
using System.Data;
using System.Threading;
using Cryptopia.Enums;
using Cryptopia.API.Utils;
using System.Transactions;

namespace Cryptopia.DepositTrackerService.Implementation
{
	/// <summary>
	/// Class for polling the wallets for deposits
	/// </summary>
	public class DepositTracker
	{
		#region Fields

		private int _pollPeriod;
		private CancellationToken _cancelToken;
		private bool _isRunning;
		private string _hostname;

		private bool _isEnabled => !_cancelToken.IsCancellationRequested;

		private readonly Log Log = LoggingManager.GetLog(typeof(DepositTrackerService));

		#endregion

		#region Constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="DepositTracker" /> class.
		/// </summary>
		/// <param name="walletData">The wallet data.</param>
		/// <param name="shortPollPeriod">The short poll period.</param>
		/// <param name="longPollPeriod">The long poll period.</param>
		/// <param name="maxRealtimeConfirmations">The maximum realtime confirmations.</param>
		public DepositTracker(CancellationToken cancelToken, int pollPeriod, string hostname)
		{
			_hostname = hostname;
			_pollPeriod = pollPeriod;
			_cancelToken = cancelToken;
			_isRunning = true;
			Log.Message(LogLevel.Info, "[Start] - Starting deposit tracker, Host: {0}, Period: {1} Seconds.", _hostname, _pollPeriod);

			Task.Factory
								.StartNew(async () => await Process().ConfigureAwait(false), cancelToken, TaskCreationOptions.LongRunning, TaskScheduler.Default)
								.ConfigureAwait(false);
		}

		#endregion

		public bool Running
		{
			get { return _isRunning; }
		}

		#region Methods

		private async Task Process()
		{
			while (_isEnabled)
			{
				try
				{
					Log.Message(LogLevel.Info, "[Process] - Processing currencies...");

					var start = DateTime.Now;
					Log.Message(LogLevel.Info, "[Process] - Processing deposits...");
					await QueryDeposits().ConfigureAwait(false);
					var elapsed = (DateTime.Now - start);
					Log.Message(LogLevel.Info, "[Process] - Processing deposits complete. Elapsed: {0}", elapsed);

					if (!_isEnabled)
						break;

					var infostart = DateTime.Now;
					Log.Message(LogLevel.Info, "[Process] - Processing info...");
					await QueryStatus().ConfigureAwait(false);
					var infoelapsed = (DateTime.Now - infostart);
					Log.Message(LogLevel.Info, "[Process] - Processing info complete. Elapsed: {0}", infoelapsed);

					var totalelapsed = (DateTime.Now - start);
					Log.Message(LogLevel.Info, "[Process] - Processing currencies complete. Elapsed: {0}", totalelapsed);

					await LogCycleCompleted().ConfigureAwait(false);

					var delay = _pollPeriod - totalelapsed.TotalSeconds;
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
				catch (Exception ex)
				{
					LogError(ex, "[Process]", "Failed in main loop. Carry on..");
				}
			}

			Log.Message(LogLevel.Info, "[Process] - Processing stopped");
			_isRunning = false;
			//_cancelToken.ThrowIfCancellationRequested();
		}

		#endregion

		private List<TransactionData> GetDeposits(Currency currency)
		{
			var deposits = GetTransactions(currency, 60000 * 5);
			if (deposits != null)
			{
				return deposits;
			}
			throw new TimeoutException();
		}


		private GetInfoData GetInfo(Currency currency)
		{
			var info = GetInfo(currency, 60000);
			if (info != null)
			{
				return info;
			}
			return null;
		}

		private List<TransactionData> GetTransactions(Currency currency, int timeout)
		{
			try
			{
				var connector = new WalletConnector(currency.WalletHost, currency.WalletPort, currency.WalletUser, currency.WalletPass, timeout);
				var deposits = new List<TransactionData>(connector.GetDeposits(currency.LastBlockHash));
				return deposits.Where(x => x.Amount > 0).ToList();
			}
			catch (Exception)
			{
				Log.Message(LogLevel.Debug, "[GetTransactions] - Timeout: {0}ms", timeout);
				return null;
			}
		}

		private GetInfoData GetInfo(Currency currency, int timeout)
		{
			try
			{
				var connector = new WalletConnector(currency.WalletHost, currency.WalletPort, currency.WalletUser, currency.WalletPass, timeout);
				return connector.GetInfo();
			}
			catch (Exception)
			{
				Log.Message(LogLevel.Debug, "[GetTransactions] - Timeout: {0}ms", timeout);
				return null;
			}
		}

		private async Task QueryDeposits()
		{
			try
			{
				using (var context = new MsSqlDataAccess())
				using (var userRepo = new Repository<User>())
				using (var currencyRepo = new Repository<Currency>())
				using (var depositRepo = new Repository<Deposit>())
				using (var addressRepo = new Repository<Address>())
				{
					// Cache some data
					var userTable = userRepo.GetAll().ToListNoLock();
					var currencies = currencyRepo.GetAll().Where(x => x.WalletHost == _hostname && x.IsEnabled).ToListNoLock();
					var currencyIds = currencies.Select(x => x.Id).ToList();
					var addresses = addressRepo.GetAll().Where(x => currencyIds.Contains(x.CurrencyId)).ToListNoLock();
					var allDeposits = depositRepo.GetAll().Where(x => currencyIds.Contains(x.CurrencyId)).ToListNoLock();
					foreach (var currency in currencies)
					{
						if (!_isEnabled)
						{
							return;
						}

						try
						{
							var start = DateTime.Now;
							Log.Message(LogLevel.Debug, "[QueryDeposits] - Processing Currency: {0}", currency.Symbol);
							if (!IsWalletOnline(currency.Status))
							{
								Log.Message(LogLevel.Info, "[QueryDeposits] - {0} wallet current status is '{1}', skipping.", currency.Symbol, currency.Status);
								continue;
							}

							// Get everything we need
							bool updateLastBlockHash = true;
							string lastBlockHash = currency.LastBlockHash;

							// Get all transactiosn since the last deposit we confirmed
							var walletDeposits = GetDeposits(currency);
							if (!walletDeposits.IsNullOrEmpty())
							{
								Log.Message(LogLevel.Debug, "[QueryDeposits] - {0} deposits found since last block.", walletDeposits.Count());
								foreach (var walletDeposit in walletDeposits.OrderBy(x => x.Time))
								{
									if (!_isEnabled)
									{
										return;
									}

									try
									{
										var user = userTable.FirstOrDefault(u => u.Id == GetUserId(walletDeposit.Account));
										if (user == null)
										{
											var address =
												addresses.FirstOrDefault(x => x.CurrencyId == currency.Id && x.AddressHash == walletDeposit.Address);
											if (address == null)
												continue;

											user = userTable.FirstOrDefault(u => u.Id == address.UserId);
											if (user == null)
												continue;
										}

										// See if it already exists
										var existingDeposit =
											allDeposits.FirstOrDefault(
												x => x.CurrencyId == currency.Id && x.Txid == walletDeposit.Txid && x.UserId == user.Id);
										if (existingDeposit == null)
										{
											// Insert new deposit
											Log.Message(LogLevel.Debug, "[QueryDeposits] - New deposit found. TxId: {0}", walletDeposit.Txid);
											var depositStatus = walletDeposit.Confirmations >= currency.MinConfirmations
												? DepositStatus.Confirmed
												: DepositStatus.UnConfirmed;
											await InsertDeposit(context, user.Id, currency.Id, walletDeposit, DepositType.Normal, depositStatus);
											await AuditUserBalance(context, user.Id, currency.Id);
											Log.Message(LogLevel.Debug, "[QueryDeposits] - New deposit inserted. TxId: {0}", walletDeposit.Txid);
											if (depositStatus == DepositStatus.Confirmed)
												lastBlockHash = walletDeposit.Blockhash;

											//await SendBalanceNotification(user.Id, currency.Id, currency.Symbol);
											continue;
										}
										else
										{
											// Chech confirmations for the unconfirmed deposits
											if (existingDeposit.DepositStatus == DepositStatus.UnConfirmed)
											{
												if (existingDeposit.Confirmations == walletDeposit.Confirmations)
												{
													continue;
												}

												// Update and check if confirmed
												Log.Message(LogLevel.Debug, "[QueryDeposits] - Updating deposit confirmations. DepositId: {0}",
													existingDeposit.Id);
												if (existingDeposit.Confirmations >= currency.MinConfirmations)
												{
													Log.Message(LogLevel.Debug, "[QueryDeposits] - Confirming Deposit. DepositId: {0}", existingDeposit.Id);

													// Depoist is now confirmed, update syatus and notify user
													await UpdateDeposit(context, existingDeposit.Id, walletDeposit.Confirmations, DepositStatus.Confirmed);
													await AuditUserBalance(context, user.Id, currency.Id);
													//await SendNotification(new WalletNotification(user.Id, string.Format("Deposit #{0} Confirmed", existingDeposit.Id), string.Format("{0} {1} has been added to your balance.", existingDeposit.Amount, currency.Symbol)));
													//await SendBalanceNotification(user.Id, currency.Id, currency.Symbol);

													lastBlockHash = walletDeposit.Blockhash;
													Log.Message(LogLevel.Debug, "[QueryDeposits] - Deposit confirmed. DepositId: {0}", existingDeposit.Id);
													continue;
												}
												await UpdateDeposit(context, existingDeposit.Id, walletDeposit.Confirmations, DepositStatus.UnConfirmed);
												continue;
											}
										}
									}
									catch (Exception ex)
									{
										// dont update the block hash on error so we can try pick up the errord deposit next poll
										updateLastBlockHash = false;
										LogError(ex, "QueryDeposits", "Failed to process deposit, Currency: {0}, TxId: {1}", currency.Symbol, walletDeposit.Txid);
									}
								}
							}

							// Update the last block we processed
							if (updateLastBlockHash && lastBlockHash != currency.LastBlockHash)
							{
								Log.Message(LogLevel.Debug, "[QueryDeposits] - Updating currency last block hash: {0}", lastBlockHash);
								await UpdateLastBlockHash(context, currency.Id, lastBlockHash);
							}

							Log.Message(LogLevel.Debug, "[QueryDeposits] - Processing Currency: {0} complete. Elapsed {1}", currency.Symbol,
								DateTime.Now - start);
						}
						catch (Exception ex)
						{
							LogError(ex, "QueryDeposits", "Failed to query currency for deposits, Currency: {0}", currency.Symbol);
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogError(ex, "QueryDeposits", "Failed to query deposits");
			}
		}

		private async Task QueryStatus()
		{
			try
			{
				using (var currencyRepo = new Repository<Currency>())
				{
					var currencies = currencyRepo.GetAll().Where(x => x.WalletHost == _hostname && x.IsEnabled).ToListNoLock();
					foreach (var currency in currencies)
					{
						if (!_isEnabled)
							break;

						if (currency.Status == CurrencyStatus.Maintenance || currency.Status == CurrencyStatus.Offline)
						{
							continue;
						}

						var currencyInfo = GetInfo(currency);
						if (currencyInfo == null)
						{
							if (currency.Status == CurrencyStatus.OK)
							{
								currency.Status = CurrencyStatus.NoConnections;
							}
							continue;
						}

						currency.Balance = currencyInfo.Balance;
						currency.Block = currencyInfo.Blocks;
						currency.Connections = currencyInfo.Connections;
						currency.Errors = currencyInfo.Errors;
						currency.Version = currencyInfo.Version;

						if (currency.Status == CurrencyStatus.NoConnections && currency.Connections > 0)
							currency.Status = CurrencyStatus.OK;
					}
					await currencyRepo.SaveOrMergeAsync();
				}
			}
			catch (Exception ex)
			{
				LogError(ex, "QueryStatus", "Failed to query currency statuses");
			}
		}

		#region Helpers

		string _updateServerInfo = "UPDATE ServerInfo SET DepositTrackerLastRun = @LastRunTime WHERE IPAddress = @Host";

		private async Task LogCycleCompleted()
		{
			try
			{
				using (var context = new MsSqlDataAccess())
				{
					Log.Message(LogLevel.Debug, "[UpdateServerInfo] - Updating timestamp");
					int rowAffected = await context.ExecuteAsync(_updateServerInfo, new
					{
						LastRunTime = DateTime.UtcNow,
						Host = _hostname
					});

					if (rowAffected > 0)
					{
						Log.Message(LogLevel.Debug, "[UpdateServerInfo] - timestamp updated");
					}
					else
					{
						Log.Message(LogLevel.Error, "[UpdateServerInfo] - timestamp update for host: {0} failed", _hostname);
					}
				}
			}
			catch (Exception ex)
			{
				LogError(ex, "[UpdateServerInfo]", "Timestamp update for host: {0} failed", _hostname);
			}
		}

		string _insertDeposit =
	@"INSERT INTO Deposit (UserId, CurrencyId, Amount, TxId, Confirmations, DepositTypeId, DepositStatusId, TimeStamp)
								VALUES (@UserId, @CurrencyId, @Amount, @TxId, @Confirmations, @DepositTypeId, @DepositStatusId, @TimeStamp)";

		private async Task<bool> InsertDeposit(IDataAccess context, Guid userId, int currencyId, TransactionData depositData,
			DepositType type, DepositStatus status)
		{
			try
			{
				Log.Message(LogLevel.Debug, "[InsertDeposit] - Inserting new deposit. CurrencyId: {0}, TxId: {1}", currencyId,
					depositData.Txid);
				int rowAffected = await context.ExecuteAsync(_insertDeposit, new
				{
					UserId = userId,
					CurrencyId = currencyId,
					Amount = depositData.Amount,
					TxId = depositData.Txid,
					Confirmations = depositData.Confirmations,
					DepositTypeId = type,
					DepositStatusId = status,
					TimeStamp = depositData.Time == 0
						? DateTime.UtcNow
						: depositData.Time.ToDateTime()
				});

				if (rowAffected > 0)
				{
					Log.Message(LogLevel.Debug, "[InsertDeposit] - New deposit inserted. CurrencyId: {0}, TxId: {1}", currencyId,
						depositData.Txid);
					return true;
				}
				Log.Message(LogLevel.Error, "[InsertDeposit] - Failed to insert deposit, CurrencyId: {0}, TxId: {1}", currencyId,
					depositData.Txid);
			}
			catch (Exception ex)
			{
				LogError(ex, "InsertDeposit", "Failed to insert deposit, CurrencyId: {0}, TxId: {1}", currencyId, depositData.Txid);
			}
			return false;
		}

		string _updateDeposit =
			"UPDATE Deposit SET DepositStatusId = @DepositStatusId, Confirmations = @Confirmations WHERE Id = @DepositId";

		private async Task<bool> UpdateDeposit(IDataAccess context, int depositId, int confirmations, DepositStatus status)
		{
			try
			{
				Log.Message(LogLevel.Debug, "[UpdateDeposit] - Updating deposit. DepositId: {0}, Status: {1} Conformations: {2}",
					depositId, status, confirmations);
				int rowAffected = await context.ExecuteAsync(_updateDeposit, new
				{
					DepositId = depositId,
					DepositStatusId = status,
					Confirmations = confirmations,
				});

				if (rowAffected > 0)
				{
					Log.Message(LogLevel.Debug, "[UpdateDeposit] - Deposit Updated. DepositId: {0}, Status: {1} Conformations: {2}",
						depositId, status, confirmations);
					return true;
				}
				Log.Message(LogLevel.Error,
					"[UpdateDeposit] - Failed to update deposit. DepositId: {0}, Status: {1} Conformations: {2}", depositId, status,
					confirmations);
			}
			catch (Exception ex)
			{
				LogError(ex, "UpdateDeposit", "Failed to update deposit to '{0}', DepositId: {1}", status, depositId);
			}
			return false;
		}


		string _updateLastBlockHash = "UPDATE Currency SET LastBlockHash = @LastBlockHash WHERE Id = @CurrencyId";

		private async Task<bool> UpdateLastBlockHash(IDataAccess context, int currencyId, string hash)
		{
			try
			{
				Log.Message(LogLevel.Debug, "[UpdateLastBlockHash] - Updating last blockhash. CurrencyId: {0}, Blockhash: {1}",
					currencyId, hash);
				int rowAffected = await context.ExecuteAsync(_updateLastBlockHash, new
				{
					CurrencyId = currencyId,
					LastBlockHash = hash
				});
				if (rowAffected > 0)
				{
					Log.Message(LogLevel.Debug, "[UpdateLastBlockHash] - Last blockhash updated. CurrencyId: {0}, Blockhash: {1}",
						currencyId, hash);
					return true;
				}
				Log.Message(LogLevel.Debug,
					"[UpdateLastBlockHash] - Failed to pdate last blockhash. CurrencyId: {0}, Blockhash: {1}", currencyId, hash);
			}
			catch (Exception ex)
			{
				LogError(ex, "UpdateLastBlockHash", "Failed to update last blockhash, CurrencyId: {0}", currencyId);
			}
			return false;
		}

		//		string _getDeposits =
		//			@"SELECT 
		//						Id,
		//						UserId,
		//						CurrencyId,
		//						Amount,
		//						Txid,
		//						Confirmations,
		//						DepositTypeId AS DepositType,
		//						DepositStatusId AS DepositStatus,
		//						TimeStamp
		//					FROM Deposit WITH(NOLOCK) 
		//					WHERE CurrencyId = @CurrencyId";
		//		private async Task<List<Deposit>> GetDeposits(IDataAccess context, int currencyId)
		//		{
		//			try
		//			{
		//				var deposits = await context.ExecuteQueryAsync<Deposit>(_getDeposits, new { CurrencyId = currencyId });
		//				if (!deposits.IsNullOrEmpty())
		//				{
		//					return new List<Deposit>(deposits);
		//				}
		//			}
		//			catch (Exception ex)
		//			{

		//			}
		//			return new List<Deposit>();
		//		}

		private async Task<bool> AuditUserBalance(IDataAccess context, Guid userId, int currency)
		{
			try
			{
				Log.Message(LogLevel.Debug, "Auditing user balance for CurrencyId: {0}, UserId: {1}...", currency, userId);
				if (
					await
						context.ExecuteAsync("AuditUserBalance", new { UserId = userId, CurrencyId = currency }, CommandType.StoredProcedure) >
					0)
				{
					Log.Message(LogLevel.Debug, "Successfully audited user balance for CurrencyId: {0}, UserId: {1}", currency, userId);
					return true;
				}
				LogError("AuditUserBalance", "Failed to audit user balance, CurrencyId: {0}, UserId: {1}", currency, userId);
			}
			catch (Exception)
			{
			}
			return false;
		}

		///// <summary>
		///// Audits the user balance.
		///// </summary>
		///// <param name="userId">The user identifier.</param>
		///// <param name="currency">The currency.</param>
		///// <returns></returns>
		//private async Task AuditUserBalance(Guid userId, int currency)
		//{
		//	using (var dataAccess = new MsSqlDataAccess())
		//	{
		//		Log.Message(LogLevel.Debug, "Auditing user balance for CurrencyId: {0}, UserId: {1}...", currency, userId);
		//		if (await dataAccess.ExecuteAsync("AuditUserBalance", new { UserId = userId, CurrencyId = currency }, CommandType.StoredProcedure) > 0)
		//		{
		//			Log.Message(LogLevel.Debug, "Successfully audited user balance for CurrencyId: {0}, UserId: {1}", currency, userId);
		//		}
		//		else
		//		{
		//			LogError("AuditUserBalance", "Failed to audit user balance, CurrencyId: {0}, UserId: {1}", currency, userId);
		//		}
		//	}
		//}


		///// <summary>
		///// Creates a new deposit.
		///// </summary>
		///// <param name="userId">The user identifier.</param>
		///// <param name="currencyId">The currency identifier.</param>
		///// <param name="depositData">The deposit data.</param>
		///// <param name="isFromPool">if set to <c>true</c> [is from pool].</param>
		///// <returns></returns>
		//private Deposit CreateDeposit(Guid userId, int currencyId, TransactionData depositData, DepositType type)
		//{
		//	var newDeposit = new Deposit();
		//	newDeposit.Amount = depositData.Amount;
		//	newDeposit.Confirmations = 0;
		//	newDeposit.CurrencyId = currencyId;
		//	newDeposit.DepositStatus = DepositStatus.UnConfirmed;
		//	newDeposit.DepositType = type;
		//	newDeposit.TimeStamp = depositData.Time.ToDateTime();
		//	newDeposit.Txid = depositData.Txid;
		//	newDeposit.UserId = userId;
		//	return newDeposit;
		//}

		/// <summary>
		/// Gets the user identifier from the wallet account name.
		/// </summary>
		/// <param name="account">The account.</param>
		/// <returns>The UserId</returns>
		private Guid _noUserGuid = Guid.Parse("3BA5106A-5F6A-4423-9E00-FCA2771249ED");

		private Guid GetUserId(string account)
		{
			Guid value = Guid.Empty;
			if (!string.IsNullOrEmpty(account) && Guid.TryParse(account, out value))
			{
				return value;
			}
			return _noUserGuid;
		}

		private bool IsWalletOnline(CurrencyStatus status)
		{
			switch (status)
			{
				case CurrencyStatus.OK:
					return true;
				case CurrencyStatus.Maintenance:
				case CurrencyStatus.NoConnections:
				case CurrencyStatus.Offline:
					return false;
				default:
					break;
			}
			return false;
		}

		/// <summary>
		/// Sends the notification.
		/// </summary>
		/// <param name="notification">The notification.</param>
		/// <returns></returns>
		private async Task SendNotification(WalletNotification notification)
		{
			try
			{
				using (var notificationService = new DepositNotifier())
				{
					await notificationService.SendNotification(NotificationLevelType.Info, notification.UserId, notification.Header, notification.Message);
				}
			}
			catch (Exception ex)
			{
				LogError(ex, "SendNotification", "Failed to send notification");
			}
		}

		private async Task SendBalanceNotification(Guid userId, int currencyId, string symbol)
		{
			try
			{
				using (var notificationService = new DepositNotifier())
				{
					await notificationService.SendDataNotification(DataNotificationType.BalanceUpdate, userId, new
					{
						CurrencyId = currencyId,
						Symbol = symbol
					});
				}
			}
			catch (Exception ex)
			{
				LogError(ex, "SendNotification", "Failed to send notification");
			}
		}

		/// <summary>
		/// Logs the error.
		/// </summary>
		/// <param name="method">The method.</param>
		/// <param name="message">The message.</param>
		/// <param name="fparams">The fparams.</param>
		private void LogError(string method, string message, params object[] fparams)
		{
			LogError(null, method, message, fparams);
		}

		/// <summary>
		/// Logs the error.
		/// </summary>
		/// <param name="ex">The exception.</param>
		/// <param name="method">The method name.</param>
		/// <param name="message">The message.</param>
		/// <param name="fparams">The format arguments.</param>
		private void LogError(Exception ex, string method, string message, params object[] fparams)
		{
			try
			{
				var logMsg = string.Concat("[", method, "] - ", string.Format(message, fparams));
				if (ex != null)
				{
					Log.Exception(logMsg, ex);
				}
				else
				{
					Log.Message(LogLevel.Error, logMsg);
				}
				using (var logRepository = new Repository<ErrorLog>())
				{
					var logmessage = logRepository.CreateInstance();
					logmessage.Component = "DepositService";
					logmessage.Method = method;
					logmessage.Request = logMsg;
					logmessage.Exception = ex != null ? ex.ToString() : string.Empty;
					logmessage.Timestamp = DateTime.UtcNow;
					logRepository.Insert(logmessage);
					logRepository.Save();
				}
			}
			catch
			{
			}
		}

		#endregion
	}

	public class WalletNotification
	{
		public Guid UserId { get; set; }
		public string Header { get; set; }
		public string Message { get; set; }

		public WalletNotification(Guid user, string header, string message)
		{
			UserId = user;
			Header = header;
			Message = message;
		}
	}

	public static class Helpers
	{
		public static List<T> ToListNoLock<T>(this IQueryable<T> query)
		{
			using (var txn = new System.Transactions.TransactionScope(
					TransactionScopeOption.RequiresNew,
					new TransactionOptions
					{
						IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
					}))
			{
				return query.ToList();
			}
		}
	}
}

