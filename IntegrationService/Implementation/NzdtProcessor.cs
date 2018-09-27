using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Data.Entity;

using Cryptopia.Base.Logging;
using Cryptopia.Common.DataContext;
using Cryptopia.Data.DataContext;
using Cryptopia.Entity;
using Cryptopia.Enums;
using Cryptopia.Infrastructure.Common.DataContext;
using Cryptopia.WalletAPI.Base;


namespace Cryptopia.IntegrationService.Implementation
{
	public class NzdtProcessor : ProcessorBase<CancellationToken>
	{
		private readonly Log _log = LoggingManager.GetLog(typeof(NzdtProcessor));
		private int _pollPeriod = 60;
		private CancellationToken _cancelToken;

		private string _nzdtAssetWalletIp;
		private int _nzdtAssetWalletPort;
		private string _nzdtAssetWalletUserName;
		private string _nzdtAssetWalletPassword;

		public NzdtProcessor(CancellationToken cancelToken) : base(cancelToken)
		{
			_cancelToken = cancelToken;
			ExchangeDataContextFactory = new ExchangeDataContextFactory();

			_nzdtAssetWalletPort = int.Parse(ConfigurationManager.AppSettings["NzdtAssetWalletPort"]);

			_nzdtAssetWalletIp = ConfigurationManager.AppSettings["NzdtAssetWalletIp"];
			_nzdtAssetWalletUserName = ConfigurationManager.AppSettings["NzdtAssetWalletUserName"];
			_nzdtAssetWalletPassword = ConfigurationManager.AppSettings["NzdtAssetWalletPassword"];

			if (string.IsNullOrEmpty(_nzdtAssetWalletIp) || string.IsNullOrEmpty(_nzdtAssetWalletUserName) || string.IsNullOrEmpty(_nzdtAssetWalletPassword))
			{
				throw new ArgumentNullException("Nzdt Asset Wallet app.config settings missing");
			}
		}

		protected override Log Log
		{
			get
			{
				return _log;
			}
		}

		public IExchangeDataContextFactory ExchangeDataContextFactory { get; set; }

		public override string StartLog { get { return "[Process] - Starting Nzdt Processor."; } }

		public override string StopLog { get { return "[Process] - Stopping Nzdt Processor."; } }

		protected override async void Process()
		{
			Log.Message(LogLevel.Info, "[Process] - Started Nzdt Processor.");

			while (_isEnabled)
			{
				try
				{
					await ProcessTransactions().ConfigureAwait(false);
					await Task.Delay(TimeSpan.FromMinutes(_pollPeriod), _cancelToken).ConfigureAwait(false);
				}
				catch (TaskCanceledException)
				{
					Log.Message(LogLevel.Info, "[Process] - Nzdt Processing canceled");
					break;
				}
				catch (Exception ex)
				{
					Log.Exception($"[ProcessNZDT] ProcessTransactions failed", ex);
					break;
				}
			}
			
			_isRunning = false;
			Log.Message(LogLevel.Info, "[Process] - Stopped Nzdt Processor.");
		}

		private async Task ProcessTransactions()
		{
			Log.Message(LogLevel.Info, "[ProcessNZDT] - Processing NZDT Transactions...");

			List<NzdtTransaction> nzdtTransactions;

			using (var context = ExchangeDataContextFactory.CreateContext())
			{
				nzdtTransactions = await context.NzdtTransaction
					.Where(x => x.TransactionStatus == NzdtTransactionStatus.ReadyForProcessing)
					.Where(x => DbFunctions.AddHours(x.CreatedOn, 1) <= DateTime.UtcNow)
					.ToListNoLockAsync();

				Log.Message(LogLevel.Info, $"[ProcessNZDT] - {nzdtTransactions.Count()} transactions found, processing...");

				foreach (var transaction in nzdtTransactions)
				{
					transaction.TransactionStatus = NzdtTransactionStatus.Processed;
				}

				await context.SaveChangesAsync();
			}

			var wallet = new WalletConnector(_nzdtAssetWalletIp, _nzdtAssetWalletPort, _nzdtAssetWalletUserName, _nzdtAssetWalletPassword, 30000);

			foreach (var transaction in nzdtTransactions)
			{
				try
				{
					var sendResult = await wallet.SendToAddressAsync(Constant.NzdtBaseExchangeAddress, transaction.Amount);

					using (var context = ExchangeDataContextFactory.CreateContext())
					{
						var deposit = new Deposit
						{
							Txid = string.IsNullOrEmpty(sendResult?.Txid) ? $"{transaction.Id}" : sendResult.Txid,
							Amount = transaction.Amount,
							TimeStamp = DateTime.UtcNow,
							CurrencyId = Constant.NZDT_ID,
							Status = DepositStatus.Confirmed,
							Confirmations = 20,
							UserId = transaction.UserId.Value,
							Type = DepositType.Normal
						};

						var tx = await context.NzdtTransaction.FirstNoLockAsync(x => x.Id == transaction.Id);
						tx.Deposit = deposit;

						await context.SaveChangesAsync();

						await context.AuditUserBalance(transaction.UserId.Value, Constant.NZDT_ID);

						await context.SaveChangesAsync();
					}
				}
				catch (Exception ex)
				{
					Log.Exception($"[ProcessNZDT] Insert Deposit failed for transaction {transaction.Id}", ex);
				}
			}

			Log.Message(LogLevel.Info, $"[ProcessNZDT] - Processing NZDT Transactions complete.");
		}

	}
}
