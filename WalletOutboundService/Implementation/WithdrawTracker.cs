using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cryptopia.API.Logging.Base;
using Cryptopia.Common.DataAccess;
using Cryptopia.Logging;
using Cryptopia.WalletAPI.Base;
using Cryptopia.WalletAPI.DataObjects;
using System.Data;
using Cryptopia.API.Objects;
using Cryptopia.API.DataAccess;
using Cryptopia.Common;
using System.Threading;
using Cryptopia.Enums;
using System.Transactions;

namespace Cryptopia.OutboundService.Implementation
{
	/// <summary>
	/// Class for polling the wallets for deposit transactions
	/// </summary>
	public class WithdrawTracker
	{
		#region Fields

		/// <summary>
		/// indicates if tracking is enabled
		/// </summary>
		private bool _isEnabled = true;

		/// <summary>
		/// The short poll period (seconds)
		/// </summary>
		private readonly int _pollPeriod;

		/// <summary>
		/// The poll confirmation period(minutes)
		/// </summary>
		private readonly int _pollConfirmationPeriod;

		/// <summary>
		/// The max confirmations to track
		/// </summary>
		private readonly int _maxConfirmations;

		/// <summary>
		/// Indicates if the tracker is running
		/// </summary>
		private bool _isRunning;

		/// <summary>
		/// The cancel token
		/// </summary>
		private CancellationToken _cancelToken;

		/// <summary>
		/// The log instance
		/// </summary>
		private readonly Log Log = LoggingManager.GetLog(typeof(WithdrawTracker));

		#endregion

		#region Constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="DepositTracker" /> class.
		/// </summary>
		/// <param name="cancelToken">The cancel token.</param>
		/// <param name="pollPeriod">The poll period.</param>
		/// <param name="maxConfirmations">The maximum confirmations.</param>
		/// <param name="isDebug">if set to <c>true</c> [is debug].</param>
		public WithdrawTracker(CancellationToken cancelToken, int pollPeriod, int pollConfirmatioPeriod, int maxConfirmations)
		{
			_pollPeriod = pollPeriod;
			_pollConfirmationPeriod = pollConfirmatioPeriod;
			_maxConfirmations = maxConfirmations;
			_cancelToken = cancelToken;
			_isRunning = true;
			Log.Message(LogLevel.Info, "[Start] - Starting withdraw tracker, Period: {0} Seconds.", _pollPeriod);
			Task.Factory.StartNew(async () => await Process(), cancelToken, TaskCreationOptions.LongRunning,TaskScheduler.Default).ConfigureAwait(false);
			//Task.Factory.StartNew(async () => await ProcessConfirmations(), cancelToken, TaskCreationOptions.LongRunning,	TaskScheduler.Default).ConfigureAwait(false);
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets a value indicating whether this <see cref="WithdrawTracker"/> is running.
		/// </summary>
		/// <value>
		///   <c>true</c> if running; otherwise, <c>false</c>.
		/// </value>
		public bool Running
		{
			get { return _isRunning; }
		}

		#endregion

		#region Methods

		/// <summary>
		/// Stops the poller
		/// </summary>
		public void Stop()
		{
			Log.Message(LogLevel.Info, "[Stop] - stopping poll loops.");
			_isEnabled = false;
		}

		/// <summary>
		/// Processes this instance.
		/// </summary>
		private async Task Process()
		{
			while (_isEnabled)
			{
				try
				{
					var start = DateTime.Now;
					Log.Message(LogLevel.Info, "[Process] - Processing withdrawals");
					await ProcessWithrawals();
					Log.Message(LogLevel.Info, "[Process] - Processing withdrawals complete. Elapsed: {0}", (DateTime.Now - start));

					Log.Message(LogLevel.Info, "[Process] - Waiting.....");
					await Task.Delay(TimeSpan.FromSeconds(_pollPeriod), _cancelToken);
				}

				catch (TaskCanceledException)
				{
					Log.Message(LogLevel.Info, "[Process] - Processing canceled");
					break;
				}
			}

			Log.Message(LogLevel.Info, "[Process] - Processing stopped");
			_isRunning = false;
		}

		private async Task ProcessConfirmations()
		{
			while (_isEnabled)
			{
				try
				{
					var start = DateTime.Now;
					Log.Message(LogLevel.Info, "[ProcessConfirmations] - Processing withdrawal confirmations");
					await UpdateConfirmations();
					Log.Message(LogLevel.Info, "[ProcessConfirmations] - Processing withdrawal confirmations complete. Elapsed: {0}",
						(DateTime.Now - start));

					Log.Message(LogLevel.Info, "[ProcessConfirmations] - Waiting.....");
					await Task.Delay(TimeSpan.FromMinutes(_pollConfirmationPeriod), _cancelToken);
				}
				catch (TaskCanceledException)
				{
					Log.Message(LogLevel.Info, "[ProcessConfirmations] - Processing canceled");
					break;
				}
			}
			Log.Message(LogLevel.Info, "[ProcessConfirmations] - Processing stopped");
		}

		#endregion

		#region Implementation

		/// <summary>
		/// Processes the withrawals.
		/// </summary>
		/// <returns></returns>
		private async Task ProcessWithrawals()
		{
			using (var dataAccess = new MsSqlDataAccess())
			using (var balanceRepo = new Repository<Balance>())
			using (var currencyRepo = new Repository<Currency>())
			{
				var start = DateTime.Now;
				Log.Message(LogLevel.Info, "[ProcessWithrawals] - Processing pending withdrawals...");
				var pendingWithdraws = GetPendingWithdraws();
				if (!pendingWithdraws.Any())
				{
					Log.Message(LogLevel.Info, "[ProcessWithrawals] - No pending withdraws found.");
					return;
				}

				Log.Message(LogLevel.Info, "[ProcessWithrawals] - {0} pending withdraws found, Processing...", pendingWithdraws.Count);
				foreach (var currency in await currencyRepo.GetAllAsync(x => x.IsEnabled))
				{
					if (!_isEnabled)
					{
						return;
					}

					try
					{
						Log.Message(LogLevel.Info, "[ProcessWithrawals] - Processing pending withdraws for {0}...", currency.Symbol);
						if (!IsWalletOnline(currency.Status))
						{
							Log.Message(LogLevel.Info, "[ProcessWithrawals] - {0} wallet current status is '{1}', skipping.", currency.Symbol,
								currency.Status);
							continue;
						}

						var wallet = new WalletConnector(currency.WalletHost, currency.WalletPort, currency.WalletUser, currency.WalletPass, 60000 * 4);
						var withdraws = pendingWithdraws.Where(x => x.CurrencyId == currency.Id);
						if (withdraws.IsNullOrEmpty())
						{
							Log.Message(LogLevel.Info, "[ProcessWithrawals] - No pending withdraws found for {0}...", currency.Symbol);
							continue; // next coin
						}

						foreach (var withdraw in withdraws)
						{
							Log.Message(LogLevel.Info, "[ProcessWithrawals] - Processing withdraw #{0}...", withdraw.Id);
							try
							{
								//await SendNotification(withdraw.UserId, "Processing {0} withdraw #{1}, {2}{0}", currency.Symbol, withdraw.Id, withdraw.Amount.ToString("F8"));
								//Run a balance order to ensure we have the correct balance information
								if (!await AuditUserBalance(dataAccess, withdraw.UserId, withdraw.CurrencyId))
								{
									continue;
								}

								// get the users balance information has enough in locked in pending
								var balance = await balanceRepo.GetOrDefaultAsync(x => x.UserId == withdraw.UserId && x.CurrencyId == withdraw.CurrencyId);
								if (balance == null || balance.Total <= 0)
								{
									//await SendErrorNotification(withdraw.UserId, "{0} withdraw #{1}, Insufficient funds.", currency.Symbol, withdraw.Id);
									LogError("ProcessWithrawals", "Withdraw Failed, {0} withdraw #{1}, Insufficient funds.", currency.Symbol, withdraw.Id);
									continue;
								}
								if (balance.Total < withdraw.Amount || balance.PendingWithdraw < withdraw.Amount || balance.PendingWithdraw > balance.Total)
								{
									//await SendErrorNotification(withdraw.UserId, "{0} withdraw #{1}, Insufficient funds.", currency.Symbol, withdraw.Id);
									LogError("ProcessWithrawals", "Withdraw Failed, {0} withdraw #{1}, Insufficient funds.", currency.Symbol, withdraw.Id);
									continue;
								}

								// again check if its a valid address
								if (!await wallet.ValidateAddressAsync(withdraw.Address))
								{
									LogError("ProcessWithrawals", "Withdraw Failed, Invalid {0} address, WithdrawId: {1}", currency.Symbol, withdraw.Id);
									continue;
								}


								//decimal amountExcludingFees = currency.WithdrawFeeType == WithdrawFeeType.Normal
								//	? withdraw.Amount - currency.WithdrawFee
								//	: withdraw.Amount - ((withdraw.Amount / 100m) * currency.WithdrawFee);
								decimal withdrawFee = GetWithdrawFee(wallet, currency, withdraw.Amount);
								decimal amountExcludingFees = withdraw.Amount - withdrawFee;
								var withdrawResult = await wallet.SendToAddressAsync(withdraw.Address, amountExcludingFees);
								if (withdrawResult == null || string.IsNullOrEmpty(withdrawResult.Txid))
								{
									LogError("ProcessWithrawals", "Withdraw Failed, Failed to send {0} transaction, WithdrawId: {1}", currency.Symbol, withdraw.Id);
									continue;
								}

								// Update the withdraw with the txid and set to completed
								if (await dataAccess.ExecuteAsync("WalletSetWithdrawTxId", new { WithdrawId = withdraw.Id, TxId = withdrawResult.Txid }, CommandType.StoredProcedure) <= 0)
								{
									LogError("ProcessWithrawals", "Withdraw Failed, Failed to update {0} withdraw transaction id, WithdrawId: {1}, TxId: {2}", currency.Symbol, withdraw.Id, withdrawResult.Txid);
									continue;
								}

								//Run a balance audit to ensure we have the correct balance information
								if (!await AuditUserBalance(dataAccess, withdraw.UserId, withdraw.CurrencyId))
								{
									continue;
								}

								//await SendNotification(withdraw.UserId, "{0} withdrawal #{1} successfully processed.", currency.Symbol, withdraw.Id);
								//await SendBalanceNotification(withdraw.UserId, currency.Id, currency.Symbol);
							}
							catch (Exception ex)
							{
								LogError(ex, "ProcessWithrawals", "An exception occured processing Currency: {0}, WithdrawId: {1}", ex, currency.Symbol, withdraw.Id);
								continue;
							}
							Log.Message(LogLevel.Info, "[ProcessWithrawals] - Processing withdraw #{0} complete.", withdraw.Id);
						}
					}
					catch (Exception ex)
					{
						LogError(ex, "ProcessWithrawals", "An exception occured processing Currency: {0}", ex, currency.Symbol);
						continue;
					}
					Log.Message(LogLevel.Info, "[ProcessWithrawals] - Processing pending withdraws for {0} complete.", currency.Symbol);
				}
				Log.Message(LogLevel.Info, "[ProcessWithrawals] - Processing pending withdrawals complete. Elapsed: {0}", DateTime.Now - start);
			}
		}

		private decimal GetWithdrawFee(WalletConnector wallet, Currency currency, decimal amount)
		{
			try
			{
				switch (currency.WithdrawFeeType)
				{
					case WithdrawFeeType.Normal:
						return currency.WithdrawFee;
					case WithdrawFeeType.Percent:
						return (amount / 100m) * currency.WithdrawFee;
					case WithdrawFeeType.Computed:
						return 0;
					default:
						break;
				}
			}
			catch (Exception ex)
			{
				LogError(ex, "GetWithdrawFee", "An exception occured processing Currency withdraw fee: {0}", ex, currency.Symbol);
			}
			return -1m;
		}

		/// <summary>
		/// Gets all the pending withdraws from the database.
		/// </summary>
		private List<WithdrawResult> GetPendingWithdraws()
		{
			Log.Message(LogLevel.Debug, "[GetPendingWithdraws] - Getting pending withdrawals..");
			using (var dataAccess = new MsSqlDataAccess())
			{
				dataAccess.DataContext.Connection.Open();
				using (var dbTransaction = dataAccess.DataContext.Connection.BeginTransaction(System.Data.IsolationLevel.Snapshot))
				{
					dataAccess.DataContext.Transaction = dbTransaction;
					try
					{
						var pendingWithdraws = dataAccess.DataContext.ExecuteQuery<WithdrawResult>("EXEC WalletGetPendingWithdraws").ToList();
						if (!pendingWithdraws.Any())
						{
							try
							{
								if (dbTransaction != null)
								{
									dbTransaction.Rollback();
								}
							}
							catch
							{
							}
							return new List<WithdrawResult>();
						}
						dbTransaction.Commit();
						Log.Message(LogLevel.Debug, "[GetPendingWithdraws] - Getting pending withdrawals complete.");
						return pendingWithdraws;
					}
					catch (Exception ex)
					{
						try
						{
							if (dbTransaction != null)
							{
								dbTransaction.Rollback();
							}
						}
						catch
						{
						}
						LogError(ex, "GetPendingWithdraws", "An exception occured in GetPendingWithdraws");
					}
				}
			}
			return new List<WithdrawResult>();
		}

		/// <summary>
		/// Updates the confirmations.
		/// </summary>
		private async Task UpdateConfirmations()
		{
			using (var currencyRepo = new Repository<Currency>())
			using (var withdrawRepo = new Repository<Withdraw>())
			{
				var withdrawals = withdrawRepo.GetAll(x => x.Confirmations < _maxConfirmations).ToListNoLock();
				if (!withdrawals.Any())
				{
					Log.Message(LogLevel.Info, "[UpdateConfirmations] - No withdrawals found with less than {0} confirmations",
						_maxConfirmations);
					return;
				}

				var currencyGroups = withdrawals.GroupBy(x => x.CurrencyId);
				foreach (var currencyWithdrawals in currencyGroups)
				{
					try
					{
						if (!_isEnabled)
							return;

						var currency = await currencyRepo.GetOrDefaultAsync(x => x.Id == currencyWithdrawals.Key && x.IsEnabled);
						if (currency == null)
						{
							Log.Message(LogLevel.Info, "[UpdateConfirmations] - CurrencyId {0} not found or is disabled, skipping.",
								currencyWithdrawals.Key);
							continue;
						}

						if (!IsWalletOnline(currency.Status))
						{
							Log.Message(LogLevel.Info, "[UpdateConfirmations] - {0} wallet current status is '{1}', skipping.",
								currency.Symbol, currency.Status);
							continue;
						}

						Log.Message(LogLevel.Info, "[UpdateConfirmations] - Updating {0} transaction confirmations", currency.Symbol);
						var wallet = new WalletConnector(currency.WalletHost, currency.WalletPort, currency.WalletUser, currency.WalletPass);
						var transactions = await wallet.GetTransactionsAsync(currency.LastWithdrawBlockHash, TransactionDataType.Withdraw);
						foreach (var withdrawal in currencyWithdrawals)
						{
							try
							{
								if (!_isEnabled)
									return;

								var transaction = transactions.FirstOrDefault(x => x.Txid == withdrawal.Txid);
								if (transaction == null)
								{
									//  LogError("UpdateConfirmations", "Withdraw transaction not found in wallet, WithdrawId: {0}, TxId: {1}", withdrawal.Id, withdrawal.Txid);
									continue;
								}

								if (withdrawal.Confirmations != transaction.Confirmations)
								{
									withdrawal.Confirmations = transaction.Confirmations;
									if (withdrawal.Confirmations >= _maxConfirmations)
									{
										currency.LastWithdrawBlockHash = transaction.Blockhash;
									}
									Log.Message(LogLevel.Debug, "[UpdateConfirmations] - Updated confirmations on transaction, Confirms: {0}, TxId: {1}", transaction.Confirmations, transaction.Txid);
								}
							}
							catch (Exception ex)
							{
								LogError(ex, "UpdateConfirmations", "An exception occured in processing {0} withdrawal, Id: {1}", currency.Symbol, withdrawal.Id);
							}
						}
						await currencyRepo.SaveOrMergeAsync();
						await withdrawRepo.SaveAsync();
						Log.Message(LogLevel.Info, "[UpdateConfirmations] - {0} transaction confirmation updates complete", currency.Symbol);
					}
					catch (Exception ex)
					{
						LogError(ex, "UpdateConfirmations", "An exception occured in processing withdrawals, CurrencyId: {0}", currencyWithdrawals.Key);
					}
				}
			}
		}

		#endregion

		#region Helpers

		/// <summary>
		/// Audits the user balance.
		/// </summary>
		/// <param name="dataAccess">The data access layer.</param>
		/// <param name="userId">The user identifier.</param>
		/// <param name="currency">The currency.</param>
		private async Task<bool> AuditUserBalance(MsSqlDataAccess dataAccess, Guid userId, int currency)
		{
			Log.Message(LogLevel.Debug, "[AuditUserBalance] - Auditing user balance for CurrencyId: {0}, UserId: {1}...",
				currency, userId);
			if (await dataAccess.ExecuteAsync("AuditUserBalance", new { UserId = userId, CurrencyId = currency }, CommandType.StoredProcedure) > 0)
			{
				Log.Message(LogLevel.Debug, "[AuditUserBalance] - Successfully audited user balance for CurrencyId: {0}, UserId: {1}", currency, userId);
				return true;
			}
			LogError("AuditUserBalance", "Failed to audit user balance, CurrencyId: {0}, UserId: {1}", currency, userId);
			return false;
		}

		/// <summary>
		/// Determines whether this wallet can send a transaction.
		/// </summary>
		/// <param name="status">The currency status.</param>
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

		///// <summary>
		///// Sends a notification.
		///// </summary>
		///// <param name="userid">The userid.</param>
		///// <param name="message">The message.</param>
		///// <param name="fparams">The fparams.</param>
		//private async Task SendNotification(Guid userid, string message, params object[] fparams)
		//{
		//	await SendNotification(NotificationLevelType.Info, userid, "Withdrawal", string.Format(message, fparams));
		//}

		///// <summary>
		///// Sends an error notification.
		///// </summary>
		///// <param name="userid">The userid.</param>
		///// <param name="message">The message.</param>
		///// <param name="fparams">The fparams.</param>
		//private async Task SendErrorNotification(Guid userid, string message, params object[] fparams)
		//{
		//	await SendNotification(NotificationLevelType.Error, userid, "Withdrawal Error", string.Format(message, fparams));
		//}

		///// <summary>
		///// Sends the notification.
		///// </summary>
		///// <param name="notification">The notification.</param>
		///// <returns></returns>
		//private async Task SendNotification(NotificationLevelType type, Guid user, string header, string message)
		//{
		//	try
		//	{
		//		using (var notificationService = new WithdrawNotifier())
		//		{
		//			await notificationService.SendNotification(type, user, header, message);
		//		}
		//	}
		//	catch (Exception ex)
		//	{
		//		LogError(ex, "SendNotification", "Failed to send notification, Header: {0}, Message: {1}", header, message);
		//	}
		//}

		//private async Task SendBalanceNotification(Guid userId, int currencyId, string symbol)
		//{
		//	try
		//	{
		//		using (var notificationService = new WithdrawNotifier())
		//		{
		//			await notificationService.SendDataNotification(DataNotificationType.BalanceUpdate, userId, new
		//			{
		//				CurrencyId = currencyId,
		//				Symbol = symbol
		//			});
		//		}
		//	}
		//	catch (Exception ex)
		//	{
		//		LogError(ex, "SendNotification", "Failed to send notification");
		//	}
		//}

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