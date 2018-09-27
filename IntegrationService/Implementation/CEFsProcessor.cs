using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Cryptopia.Base.Logging;
using Cryptopia.Common.DataContext;
using Cryptopia.Data.DataContext;
using Cryptopia.Entity;
using Cryptopia.Enums;
using Cryptopia.Infrastructure.Common.DataContext;
using Cryptopia.WalletAPI.Base;

namespace Cryptopia.IntegrationService.Implementation
{
	public class CEFSProcessor : ProcessorBase<CancellationToken>
	{
		#region Private Members

		private readonly Log _log = LoggingManager.GetLog(typeof(CEFSProcessor));

		private const int DELAY_HOURS = 24;
		private const decimal TOTAL_PORTIONS = 6300;
		private const int WALLET_TIMEOUT = 60000 * 5;
		private const decimal FOUR_POINT_FIVE_PERCENT = 0.045M;

		private CancellationToken _token;

		#endregion

		public CEFSProcessor(CancellationToken cancelToken) : base(cancelToken)
		{
			_token = cancelToken;
			DataContextFactory = new DataContextFactory();
			ExchangeDataContextFactory = new ExchangeDataContextFactory();
		}

		#region Public Properties

		protected override Log Log
		{
			get
			{
				return _log;
			}
		}


		public IExchangeDataContextFactory ExchangeDataContextFactory { get; set; }
		public IDataContextFactory DataContextFactory { get; set; }


		public override string StartLog => "[Start] - Starting CEFS processor.";
		public override string StopLog => "[Start] - Stopping CEFS processor.";

		#endregion

		#region Public Methods

		public decimal TruncateDecimal(decimal target, int precision)
		{
			decimal step = (decimal)Math.Pow(10, precision);
			decimal tmp = Math.Truncate(step * target);
			var result = tmp / step;
			return result;
		}

		public async Task CalculateFeeTotalsAndPortions(DateTime startOfMonthToProcess, Dictionary<int, decimal> tradeHistoryFeeByCurrencyTotals, Dictionary<int, decimal> paytopiaPaymentFeeByCurrencyTotals, Dictionary<int, decimal> tradeHistoryPortionsOfCurrency, Dictionary<int, decimal> paytopiaPaymentPortionsOfCurrency)
		{
			Log.Message(LogLevel.Info, "CalculateFeeTotalsAndPortions entered.");
			List<string> auditMessages = new List<string>();
			auditMessages.Add($"CEFS Process. Calculating total fees and portion sizes for CEFS holders.");

			// get trade histories from the start of last month to the start of this month
			// (i.e. 1 Oct (incl) - 1 Nov (excl)).
			var startOfFollowingMonth = startOfMonthToProcess.AddMonths(1).Date;

			using (var context = ExchangeDataContextFactory.CreateReadOnlyContext())
			{

				var tradeHistoryFeesByCurrency = await (from h in context.TradeHistory
												join tp in context.TradePair on h.TradePairId equals tp.Id
												join c in context.Currency on tp.CurrencyId2 equals c.Id
												where h.Timestamp < startOfFollowingMonth
												&& h.Timestamp >= startOfMonthToProcess
												group h.Fee by c.Id into g
												select new { CurrencyId = g.Key, TotalFees = g.Sum() * 2 }).ToListAsync(); // multiplied by two to account for both sides of the trade

				foreach (var history in tradeHistoryFeesByCurrency)
				{
					var tradeHistoryPortionSize = (history.TotalFees * FOUR_POINT_FIVE_PERCENT) / TOTAL_PORTIONS;
					tradeHistoryPortionsOfCurrency.Add(history.CurrencyId, tradeHistoryPortionSize);
					tradeHistoryFeeByCurrencyTotals.Add(history.CurrencyId, history.TotalFees);

					// add auditing to be able to check if there is anything wrong after deployment.
					auditMessages.Add($"Trade History Total for {history.CurrencyId}: {history.TotalFees}");
					auditMessages.Add($"Trade History portion size for {history.CurrencyId}: {tradeHistoryPortionSize}");
				}
			}

			using (var hubContext = DataContextFactory.CreateContext())
			{

				var paytopiaPaymentsByCurrency = await (from p in hubContext.PaytopiaPayments
													join pi in hubContext.PaytopiaItems on p.PaytopiaItemId equals pi.Id
													where p.Timestamp < startOfFollowingMonth
													&& p.Timestamp >= startOfMonthToProcess
													&& p.Status == PaytopiaPaymentStatus.Complete
													&& pi.CurrencyId == 2
													group pi.Price by pi.CurrencyId into g
													select new { CurrencyId = g.Key, TotalPayments = g.Sum() }).ToListAsync();

				var paytopiaRefundsByCurrency = await (from p in hubContext.PaytopiaPayments
												join pi in hubContext.PaytopiaItems on p.PaytopiaItemId equals pi.Id
												where p.Timestamp < startOfFollowingMonth
												&& p.Timestamp >= startOfMonthToProcess
												&& p.Status == PaytopiaPaymentStatus.Refunded
												&& pi.CurrencyId == 2
												group pi.Price by pi.CurrencyId into g
												select new { CurrencyId = g.Key, TotalRefunds = g.Sum() }).ToListAsync();

				foreach (var payment in paytopiaPaymentsByCurrency)
				{
					var refund = paytopiaRefundsByCurrency.FirstOrDefault(x => x.CurrencyId == payment.CurrencyId);
					decimal total = 0;

					if (refund == null)
						total = payment.TotalPayments;
					else
						total = payment.TotalPayments - refund.TotalRefunds;

					var portionSize = (total * FOUR_POINT_FIVE_PERCENT) / TOTAL_PORTIONS;
					paytopiaPaymentPortionsOfCurrency.Add(payment.CurrencyId, portionSize);
					paytopiaPaymentFeeByCurrencyTotals.Add(payment.CurrencyId, total);

					auditMessages.Add($"Paytopia Payments Total for {payment.CurrencyId}: {payment.TotalPayments}");

					if (refund != null)
						auditMessages.Add($"Paytopia Payment refunds total for {payment.CurrencyId}: {refund.TotalRefunds}");

					auditMessages.Add($"Paytopia Payment portion size for {payment.CurrencyId}: {portionSize}");
				}

				auditMessages.Add($"CEFS Process. Calculation complete.");

				foreach (var message in auditMessages)
					hubContext.LogActivity(Constant.SYSTEM_USER_CEFS.ToString(), message);

				await hubContext.SaveChangesAsync();
			}

			Log.Message(LogLevel.Info, "CalculateFeeTotalsAndPortions exited.");
		}

		#endregion

		#region Protected Methods

		protected override async void Process()
		{
			Log.Message(LogLevel.Info, "[Process] - Process() Start.");
			while (_isEnabled)
			{
				try
				{
					var now = DateTime.UtcNow;
					var thisMonth = new DateTime(now.Year, now.Month, 1);

					DateTime lastMonthProcessed;

					using (var context = DataContextFactory.CreateReadOnlyContext())
					{
						Settings settings = await context.Settings.FirstOrDefaultNoLockAsync();

						if (settings == null)
						{
							Log.Message(LogLevel.Error, "No Settings object found in database. Some retard forgot to sort it out....");
							throw new NullReferenceException("No Settings object found in database. Some retard forgot to sort it out....");
						}

						lastMonthProcessed = settings.CEFSRound;
					}

					var lastMonthThatCanBeProcessed = thisMonth.AddMonths(-1);

					while (lastMonthThatCanBeProcessed > lastMonthProcessed)
					{
						var theStartOfTheMonthToBeProcessed = lastMonthProcessed.AddMonths(1);
						await ProcessInternal(theStartOfTheMonthToBeProcessed);

						using (var context = DataContextFactory.CreateContext())
						{
							var settingObj = await context.Settings.FirstOrDefaultNoLockAsync();
							settingObj.CEFSRound = lastMonthProcessed = theStartOfTheMonthToBeProcessed;
							await context.SaveChangesAsync();
						}
					}
				}
				catch (TaskCanceledException)
				{
					Log.Message(LogLevel.Info, "[Process] - Process() canceled");
					break;
				}
				catch (Exception ex)
				{
					Log.Message(LogLevel.Info, "[Process] - Process() threw exception.");
					Log.Message(LogLevel.Error, ex.Message, ex);
					// todo: send email when this happens....
					break;
				}

				await Task.Delay(TimeSpan.FromHours(DELAY_HOURS), _token).ConfigureAwait(false);
			}

			_isRunning = false;
			Log.Message(LogLevel.Info, "[Process] - Process() stopped.");
		}

		#endregion

		#region Private Methods

		private async Task ProcessInternal(DateTime startOfMonthToProcess)
		{
			Log.Message(LogLevel.Info, $"[Process] - CEFS processing round {startOfMonthToProcess.Date} started.");
			List<string> transactionIds = new List<string>();
			Dictionary<int, decimal> tradeHistoryFeeByCurrencyTotals = new Dictionary<int, decimal>();
			Dictionary<int, decimal> paytopiaPaymentFeeByCurrencyTotals = new Dictionary<int, decimal>();
			Dictionary<int, decimal> tradeHistoryPortionsOfCurrency = new Dictionary<int, decimal>();
			Dictionary<int, decimal> paytopiaPaymentPortionsOfCurrency = new Dictionary<int, decimal>();

			using (var context = ExchangeDataContextFactory.CreateContext())
			{
				// get users and balances for each user
				var userIdsAndBalances = await GetUsersAndTheirBalanacesForCEFPayments(startOfMonthToProcess, context);

				// if we have some users, do the CEFS processing routine.
				if (userIdsAndBalances.Any())
				{
					await CalculateFeeTotalsAndPortions(startOfMonthToProcess, tradeHistoryFeeByCurrencyTotals, paytopiaPaymentFeeByCurrencyTotals, tradeHistoryPortionsOfCurrency, paytopiaPaymentPortionsOfCurrency);

					TransferSumsToSystemStagingUser(transactionIds, tradeHistoryFeeByCurrencyTotals, paytopiaPaymentFeeByCurrencyTotals, context);
					List<int> Currencies = GetDistinctCurrencyIds(tradeHistoryPortionsOfCurrency, paytopiaPaymentPortionsOfCurrency);

					List<TransferHistory> transfers = CreateTransferHistoriesForUsersToReceiveCEFSPayments(tradeHistoryPortionsOfCurrency, paytopiaPaymentPortionsOfCurrency, userIdsAndBalances);

					Log.Message(LogLevel.Info, "Committing CEFS calculations to database.");
					await SaveTransfersAndAuditAllUserBalances(userIdsAndBalances, transfers, Currencies, context);
					await PerformWalletTransactionsAndUpdateDepositsWithIds(transactionIds, context);
				}
			}

			Log.Message(LogLevel.Info, "CEFS Processing round complete.");
		}

		private async Task<Dictionary<Guid, CEFSBalance>> GetUsersAndTheirBalanacesForCEFPayments(DateTime startOfMonthToProcess, IExchangeDataContext context)
		{
			Log.Message(LogLevel.Info, "GetUsersForCEFPayments entered.");

			List<Guid> userIds;
			Dictionary<Guid, CEFSBalance> results = new Dictionary<Guid, CEFSBalance>();

			var tradePairs = await context.TradePair.Where(x => x.CurrencyId1 == Constant.CEFS_ID).Select(x => x.Id).ToListAsync();

			var startOfFollowingMonth = startOfMonthToProcess.AddMonths(1);

			// Get all CEFS data from the begining of time until the end of the last month;
			var allTransfers = await context.Transfer.Where(x => x.CurrencyId == Constant.CEFS_ID && x.Timestamp < startOfFollowingMonth).ToListAsync();
			var allDeposits = await context.Deposit.Where(x => x.CurrencyId == Constant.CEFS_ID && x.TimeStamp < startOfFollowingMonth).ToListAsync();
			var allWithdraw = await context.Withdraw.Where(x => x.CurrencyId == Constant.CEFS_ID && x.TimeStamp < startOfFollowingMonth).ToListAsync();
			var allTrades = await context.Trade.Where(x => (x.Status == TradeStatus.Pending || x.Status == TradeStatus.Partial) && tradePairs.Contains(x.TradePairId) && x.Timestamp < startOfFollowingMonth).ToListAsync();
			var allTradeHistory = await context.TradeHistory.Where(x => tradePairs.Contains(x.TradePairId) && x.Timestamp < startOfFollowingMonth).ToListAsync();

			// Get users who have been involved with CEFS
			userIds = new List<Guid>()
				.Concat(allTransfers.Select(x => x.UserId))
				.Concat(allTransfers.Select(x => x.ToUserId))
				.Concat(allDeposits.Select(x => x.UserId))
				.Concat(allWithdraw.Select(x => x.UserId))
				.Concat(allTrades.Select(x => x.UserId))
				.Concat(allTradeHistory.Select(x => x.UserId))
				.Concat(allTradeHistory.Select(x => x.ToUserId))
				.Distinct()
				.ToList();

			foreach (var id in userIds)
			{
				CEFSBalance balance = new CEFSBalance();

				decimal totalDepositsConfirmed = allDeposits.Where(x => x.UserId == id && x.Status == DepositStatus.Confirmed).Sum(x => x.Amount);
				decimal totalDepositsUnconfirmed = allDeposits.Where(x => x.UserId == id && x.Status == DepositStatus.UnConfirmed).Sum(x => x.Amount);
				decimal totalWithdraw = allWithdraw.Where(x => x.UserId == id && x.Status == WithdrawStatus.Complete).Sum(x => x.Amount);
				decimal totalPendingWithdraw = allWithdraw.Where(x => x.UserId == id && (x.Status != WithdrawStatus.Complete && x.Status != WithdrawStatus.Canceled)).Sum(x => x.Amount);
				decimal totalBuy = allTradeHistory.Where(x => x.UserId == id).Sum(x => x.Amount);
				decimal totalSell = allTradeHistory.Where(x => x.ToUserId == id).Sum(x => x.Amount);

				decimal totalBuyFee = allTradeHistory.Where(x => x.UserId == id).Sum(x => x.Fee);
				decimal totalSellFee = allTradeHistory.Where(x => x.ToUserId == id).Sum(x => x.Fee);
				decimal totalBuyBase = allTradeHistory.Where(x => x.UserId == id).Sum(x => x.Rate);
				decimal totalSellBase = allTradeHistory.Where(x => x.ToUserId == id).Sum(x => x.Rate);

				decimal heldForOrders = allTrades.Where(x => x.UserId == id && x.Type == TradeHistoryType.Sell).Sum(x => x.Remaining);
				decimal heldForOrdersBase = allTrades.Where(x => x.UserId == id && x.Type == TradeHistoryType.Buy).Sum(x => (x.Remaining * x.Rate) + x.Fee);

				decimal totalTransferIn = allTransfers.Where(x => x.ToUserId == id).Sum(x => x.Amount);
				decimal totalTransferOut = allTransfers.Where(x => x.UserId == id).Sum(x => x.Amount);

				/*
					The following section has commented out variables because these are only applicable 
					to currencies that have a base currency.
					If you are copying this code to use elsewhere, be advised that these variables may be needed 
					to create a robust and correct calculation.
					*/

				decimal totalTradeIn = (totalBuy + 0 /*totalBuyBase*/);
				decimal totalTradeOut = (totalSell + 0/*totalSellBase*/ + 0/*totalBuyFee */+ 0/*totalSellFee*/);
				decimal totalIn = (totalDepositsConfirmed + totalDepositsUnconfirmed + totalTradeIn + totalTransferIn);
				decimal totalOut = (totalWithdraw + totalTradeOut + totalTransferOut);
				decimal totalBalance = (totalIn - totalOut);
				decimal totalHeldForOrders = (heldForOrders + 0/*heldForOrdersBase*/);

				balance.UserId = id;
				balance.Total = totalBalance;
				balance.Unconfirmed = totalDepositsUnconfirmed;
				balance.HeldForOrders = totalHeldForOrders;
				balance.PendingWithdrawal = totalPendingWithdraw;

				if (balance.Total > 0)
					results.Add(id, balance);
			}

			Log.Message(LogLevel.Info, "GetUsersForCEFPayments exited.");

			return results;
		}

		private void TransferSumsToSystemStagingUser(List<string> transactionIds, Dictionary<int, decimal> tradeHistoryFeeByCurrencyTotals, Dictionary<int, decimal> paytopiaPaymentFeeByCurrencyTotals, IExchangeDataContext context)
		{
			Log.Message(LogLevel.Info, "TransferSumsToSystemStagingUser entered.");

			// deposit totals into system staging user wallets in preparation for transfer to all users....
			// both calls update the context
			transactionIds.AddRange(DepositTradeHistoryTotalToSystemUser(context, tradeHistoryFeeByCurrencyTotals));
			transactionIds.AddRange(TransferPaytopiaTotalToSystemUser(context, paytopiaPaymentFeeByCurrencyTotals));

			Log.Message(LogLevel.Info, "TransferSumsToSystemStagingUser exited.");
		}

		private List<int> GetDistinctCurrencyIds(Dictionary<int, decimal> tradeHistoryPortionsOfCurrency, Dictionary<int, decimal> paytopiaPaymentPortionsOfCurrency)
		{
			List<int> Currencies = tradeHistoryPortionsOfCurrency.Keys.ToList();
			Currencies.AddRange(paytopiaPaymentPortionsOfCurrency.Keys.Except(Currencies));
			return Currencies;
		}

		private List<string> DepositTradeHistoryTotalToSystemUser(IExchangeDataContext context, Dictionary<int, decimal> totals)
		{
			Log.Message(LogLevel.Info, "DepositTradeHistoryTotalToSystemUser entered.");

			List<string> transactionIds = new List<string>();

			foreach (var kvp in totals)
			{
				Guid txId = Guid.NewGuid();
				context.Deposit.Add(new Deposit()
				{
					CurrencyId = kvp.Key,
					Amount = kvp.Value,
					UserId = Constant.SYSTEM_USER_STAGING,
					Status = DepositStatus.Confirmed,
					Txid = txId.ToString(),
					TimeStamp = DateTime.UtcNow
				});

				transactionIds.Add(txId.ToString());
			}

			Log.Message(LogLevel.Info, "DepositTradeHistoryTotalToSystemUser exited.");

			return transactionIds;
		}

		private List<string> TransferPaytopiaTotalToSystemUser(IExchangeDataContext context, Dictionary<int, decimal> totals)
		{
			Log.Message(LogLevel.Info, "TransferPaytopiaTotalToSystemUser entered.");

			List<string> transactionIds = new List<string>();

			foreach (var kvp in totals)
			{
				Guid txId = Guid.NewGuid();
				context.Transfer.Add(new TransferHistory()
				{
					CurrencyId = kvp.Key,
					Amount = kvp.Value,
					UserId = Constant.SYSTEM_USER_PAYTOPIA,
					TransferType = TransferType.Paytopia,
					ToUserId = Constant.SYSTEM_USER_STAGING,
					Timestamp = DateTime.UtcNow
				});

				transactionIds.Add(txId.ToString());
			}

			Log.Message(LogLevel.Info, "TransferPaytopiaTotalToSystemUser exited.");

			return transactionIds;
		}

		private List<TransferHistory> CreateTransferHistoriesForUsersToReceiveCEFSPayments(Dictionary<int, decimal> tradeHistoryPortionsOfCurrency, Dictionary<int, decimal> paytopiaPaymentPortionsOfCurrency, Dictionary<Guid, CEFSBalance> userIds)
		{
			Log.Message(LogLevel.Info, "CreateTransferHistoriesForUsersToReceiveCEFSPayments entered.");
			List<TransferHistory> transfers = new List<TransferHistory>();
			foreach (var balance in userIds.Values)
			{
				var totalCefs = balance.Total;
				Dictionary<int, decimal> historyPaymentAmounts = new Dictionary<int, decimal>();
				Dictionary<int, decimal> paytopiaPaymentAmounts = new Dictionary<int, decimal>();

				foreach (var portion in tradeHistoryPortionsOfCurrency)
				{
					historyPaymentAmounts.Add(portion.Key, TruncateDecimal((portion.Value * totalCefs), 8));
				}

				foreach (var portion in paytopiaPaymentPortionsOfCurrency)
				{
					paytopiaPaymentAmounts.Add(portion.Key, TruncateDecimal((portion.Value * totalCefs), 8));
				}

				foreach (var historyCEFPayment in historyPaymentAmounts)
				{
					transfers.Add(new TransferHistory()
					{
						Amount = historyCEFPayment.Value,
						CurrencyId = historyCEFPayment.Key,
						EstimatedPrice = 0,
						UserId = Constant.SYSTEM_USER_CEFS,
						ToUserId = balance.UserId,
						TransferType = TransferType.CEFS,
						Timestamp = DateTime.UtcNow,
						Fee = 0
					});
				}

				foreach (var paytopiaCEFPayment in paytopiaPaymentAmounts)
				{
					transfers.Add(new TransferHistory()
					{
						Amount = paytopiaCEFPayment.Value,
						CurrencyId = paytopiaCEFPayment.Key,
						EstimatedPrice = 0,
						UserId = Constant.SYSTEM_USER_CEFS,
						ToUserId = balance.UserId,
						TransferType = TransferType.CEFS,
						Timestamp = DateTime.UtcNow,
						Fee = 0
					});
				}
			}

			Log.Message(LogLevel.Info, "CreateTransferHistoriesForUsersToReceiveCEFSPayments exited.");
			return transfers;
		}

		private async Task SaveTransfersAndAuditAllUserBalances(Dictionary<Guid, CEFSBalance> userIds, List<TransferHistory> transfers, List<int> Currencies, IExchangeDataContext context)
		{
			Log.Message(LogLevel.Info, "SaveTransfersAndSuditUserBalances entered.");

			await context.Database.BulkInsertAsync(transfers);
			await context.SaveChangesAsync();

			// audit all users for all currencies involved.
			foreach (var currencyId in Currencies)
			{
				foreach (var id in userIds.Keys)
					await context.AuditUserBalance(id, currencyId);

				await context.AuditUserBalance(Constant.SYSTEM_USER_PAYTOPIA, currencyId);
				await context.AuditUserBalance(Constant.SYSTEM_USER_STAGING, currencyId);
				await context.AuditUserBalance(Constant.SYSTEM_USER_CEFS, currencyId);
			}

			await context.SaveChangesAsync();

			Log.Message(LogLevel.Info, "SaveTransfersAndSuditUserBalances exited.");
		}

		private async Task PerformWalletTransactionsAndUpdateDepositsWithIds(List<string> transactionIds, IExchangeDataContext context)
		{
			Log.Message(LogLevel.Info, "PerformWalletTransactionsAndUpdateDepositsWithIds entered.");

			// send transactions to the wallets for System_User cefs payments and update the deposits with the real txId.
			var cefsDeposits = await context.Deposit.Where(x => transactionIds.Contains(x.Txid)).ToListAsync().ConfigureAwait(false);
			foreach (var txId in transactionIds)
			{
				try
				{
					var deposit = cefsDeposits.FirstOrDefault(x => x.Txid == txId);
					var currency = await context.Currency.FirstOrDefaultAsync(c => c.Id == deposit.CurrencyId).ConfigureAwait(false);
					var currencyAddress = await context.Address.FirstOrDefaultAsync(a => a.UserId == Constant.SYSTEM_USER_STAGING && a.CurrencyId == deposit.CurrencyId).ConfigureAwait(false);

					if (currencyAddress == null)
					{
						Log.Message(LogLevel.Error, $"No address exists for currency {currency.Symbol}. Unable to process CEF deposit.");
						continue;
					}

					var connector = new WalletConnector(currency.WalletHost, currency.WalletPort, currency.WalletUser, currency.WalletPass, WALLET_TIMEOUT);
					var result = await connector.SendToAddressAsync(currencyAddress.AddressHash, deposit.Amount);

					if (result != null)
					{
						deposit.Txid = result.Txid;
						await context.SaveChangesAsync();
					}
				}
				catch (Exception ex)
				{
					Log.Message(LogLevel.Error, $"Wallet Transaction blew up.\r\n{ex.Message}");
				}
			}

			Log.Message(LogLevel.Info, "PerformWalletTransactionsAndUpdateDepositsWithIds exited.");
		}

		#endregion

		#region Private Classes

		private class CEFSBalance
		{
			public Guid UserId { get; set; }
			public decimal Total { get; set; }
			public decimal Unconfirmed { get; set; }
			public decimal HeldForOrders { get; set; }
			public decimal PendingWithdrawal { get; set; }
		}

		#endregion
	}
}
