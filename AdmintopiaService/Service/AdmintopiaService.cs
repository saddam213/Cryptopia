using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using AdmintopiaService.DataObjects;
using Cryptopia.Base;
using Cryptopia.Base.Logging;
using Cryptopia.Common.DataContext;
using Cryptopia.Data.DataContext;
using Cryptopia.Infrastructure.Common.DataContext;
using Cryptopia.WalletAPI.Base;
using Cryptopia.WalletAPI.DataObjects;
using Cryptopia.WalletAPI.Helpers;
using Cryptopia.Infrastructure.Incapsula.Client;
using Cryptopia.Infrastructure.Incapsula.Common.Classes;
using Cryptopia.Infrastructure.Incapsula.Common.Enums;
using Cryptopia.Infrastructure.Common.Results;
using Cryptopia.Infrastructure.Incapsula.Common.Extensions;
using System.Collections.ObjectModel;
using System.Threading;

namespace AdmintopiaService.Service
{
	/// <summary>
	///     Service to interace with the wallets for administration purposes.
	/// </summary>
	[ServiceBehavior]
	public class AdmintopiaService : IAdmintopiaService
	{
        private readonly IExchangeDataContextFactory _exchangeDataContextFactory = new ExchangeDataContextFactory();
        private readonly IMonitoringDataContextFactory _monitoringDataContextFactory = new MonitoringDataContextFactory();
		private readonly Log _log = LoggingManager.GetLog(typeof(AdmintopiaService));
        private readonly IIncapsulaClient _incapsulaClient = new IncapsulaClient();

        public AdmintopiaService()
        { }

        #region Public methods

        public void Stop()
        { }

        public async Task<List<WalletTransaction>> GetWalletTransactions(TransactionDataType transactionDataType, int currencyId, int walletTimeoutMinutes)
		{
			try
			{
				using (var context = _exchangeDataContextFactory.CreateReadOnlyContext())
				{
					var currency = await context.Currency.FirstOrDefaultNoLockAsync(c => c.Id == currencyId);
					var walletConnection = new WalletConnector(currency.WalletHost, currency.WalletPort, currency.WalletUser, currency.WalletPass, Math.Max(walletTimeoutMinutes, 1) * 60000);
					var walletTransactions = await walletConnection.GetTransactionsAsync("", transactionDataType);
					return walletTransactions.Select(x => new WalletTransaction
						{
							Timestamp = x.Time.ToDateTime(),
							Account = x.Account,
							Amount = Math.Abs(x.Amount),
							Txid = x.Txid,
							Type = Extensions.ToTransactionType(x.Category),
							Address = x.Address,
							Confirmations = x.Confirmations
						})
						.OrderByDescending(x => x.Timestamp).ToList();
				}
			}
			catch (Exception e)
			{
				_log.Exception("[GetWalletTransactions] - An exception occured while Loading Wallet Transactions", e);
			}

			return new List<WalletTransaction>();
		}

        public async Task<List<WalletTransaction>> GetWalletTransactionsSince(TransactionDataType transactionDataType, int currencyId, int walletTimeoutMinutes, int searchBlockLength)
        {
            try
            {
                using (var context = _exchangeDataContextFactory.CreateReadOnlyContext())
                {
                    var currency = await context.Currency.FirstOrDefaultNoLockAsync(c => c.Id == currencyId);
                    var walletConnection = new WalletConnector(currency.WalletHost, currency.WalletPort, currency.WalletUser, currency.WalletPass, Math.Max(walletTimeoutMinutes, 1) * 60000);
                    var blockHash = await walletConnection.GetBlockHashAsync(searchBlockLength);
                    var walletTransactions = await walletConnection.GetTransactionsAsync(blockHash, transactionDataType);
                    return walletTransactions.Select(x => new WalletTransaction
                    {
                        Timestamp = x.Time.ToDateTime(),
                        Account = x.Account,
                        Amount = Math.Abs(x.Amount),
                        Txid = x.Txid,
                        Type = Extensions.ToTransactionType(x.Category),
                        Address = x.Address,
                        Confirmations = x.Confirmations
                    })
                    .OrderByDescending(x => x.Timestamp).ToList();
                }
            }
            catch (Exception e)
            {
                _log.Exception("[GetWalletTransactions] - An exception occured while Loading Wallet Transactions", e);
            }

            return new List<WalletTransaction>();
        }

        public async Task<IPBlacklist> BlacklistIpAddress(string ipAddress)
        {
            IPBlacklist blackList = await _incapsulaClient.BlacklistIp(ipAddress);
            return blackList;
        }

        public async Task<IPBlacklist> GetIpAddressBlacklist()
        {
            return await _incapsulaClient.GetIpBlacklist();
        }

        public async Task<ResponseCode> PurgeSiteCache()
        {
            return await _incapsulaClient.PurgeSiteCache();
        }

        #endregion
    }
}