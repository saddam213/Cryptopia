using System;
using System.ServiceModel;
using System.Threading.Tasks;
using Cryptopia.API.Logging.Base;
using Cryptopia.Logging;
using Cryptopia.API.Objects;
using Cryptopia.API.DataAccess;
using Cryptopia.WalletAPI.Base;
using Cryptopia.WalletAPI.DataObjects;
using System.Collections.Generic;
using Cryptopia.InboundService.DataObjects;

namespace Cryptopia.InboundService
{
	/// <summary>
	/// Service to interace with the wallets, Create addresses etc.
	/// </summary>
	[ServiceBehavior]
	public class WalletInboundService : IWalletInbound
	{
		/// <summary>
		/// The logging instance
		/// </summary>
		private readonly Log Log = LoggingManager.GetLog(typeof(WalletInboundService));

		/// <summary>
		/// Creates an address for the specified wallet with the userid.
		/// </summary>
		/// <param name="walletId">The wallet id.</param>
		/// <param name="userId">The user id.</param>
		/// <returns>the new address and private key created in the wallet</returns>
		public async Task<string[]> CreateAddress(int walletId, Guid userId)
		{
			try
			{
				using (var currencyRepo = new Repository<Currency>())
				{
					var currency = await currencyRepo.GetOrDefaultAsync(x => x.Id == walletId);
					if (currency == null)
					{
						return null;
					}

					//var wallet = new WalletConnector("127.0.0.1", 33113, "sa_ddam213.3", "213bit");
					var wallet = new WalletConnector(currency.WalletHost, currency.WalletPort, currency.WalletUser, currency.WalletPass);
					if (wallet == null)
					{
						Log.Message(LogLevel.Error, "[CreateAddress] - Wallet '{0}' not found.", walletId);
						return null;
					}

					string address = wallet.GenerateAddress(userId.ToString());
					if (string.IsNullOrEmpty(address))
					{
						Log.Message(LogLevel.Error, "[CreateAddress] - Wallet '{0}' failed to generate address.", walletId);
						return null;
					}
					Log.Message(LogLevel.Debug, "[CreateAddress] - Address '{0}' generated for CurrencyId: {1}, UserId: {2}", address, walletId, userId);

					string privateKey = wallet.DumpPrivKey(address);
					if (string.IsNullOrEmpty(privateKey))
					{
						Log.Message(LogLevel.Error, "[CreateAddress] - Wallet '{0}' failed to dump privatekey.", walletId);
						return null;
					}
					Log.Message(LogLevel.Debug, "[CreateAddress] - Private key '{0}' dumped for CurrencyId: {1}, UserId: {2}", privateKey, walletId, userId);

					Log.Message(LogLevel.Info, "[CreateAddress] - New address '{0}' created for CurrencyId: {1}, UserId: {2}", address, walletId, userId);
					return new string[] { address, privateKey };
				}
			}
			catch (Exception ex)
			{
				Log.Exception("[CreateAddress] - An exception occured creating address, WalletId: {0}, UserId: {1}", ex, walletId, userId);
			}
			return null;
		}

		/// <summary>
		/// Validates the address against the wallet.
		/// </summary>
		/// <param name="walletId">The wallet id.</param>
		/// <param name="address">The address.</param>
		/// <returns>true if the address is a valid address for the wallet, otherwise false</returns>
		public async Task<bool> ValidateAddress(int walletId, string address)
		{
			try
			{

				using (var currencyRepo = new Repository<Currency>())
				{
					var currency = await currencyRepo.GetOrDefaultAsync(x => x.Id == walletId);
					if (currency == null)
					{
						return false;
					}

					var wallet = new WalletConnector(currency.WalletHost, currency.WalletPort, currency.WalletUser, currency.WalletPass);
					if (wallet == null)
					{
						Log.Message(LogLevel.Error, "[CreateAddress] - Wallet '{0}' not found.", walletId);
						return false;
					}

					if (!wallet.ValidateAddress(address))
					{
						Log.Message(LogLevel.Error, "[ValidateAddress] - Address '{0}' is not a valid address for CurrencyId: {1}", address, walletId);
						return false;
					}

					Log.Message(LogLevel.Info, "[ValidateAddress] - Address '{0}' successfully validated for CurrencyId: {1}", address, walletId);
					return true;
				}
			}
			catch (Exception ex)
			{
				Log.Exception("[ValidateAddress] - An exception occured validating address, WalletId: {0}, Address: {1}", ex, walletId, address);
			}
			return false;
		}

		public async Task<GetWalletTransactionResponse> GetTransaction(GetWalletTransactionRequest request)
		{
			using (var currencyRepo = new Repository<Currency>())
			{
				var currency = await currencyRepo.GetOrDefaultAsync(x => x.Id == request.CurrencyId);
				if (currency == null)
				{
					Log.Message(LogLevel.Error, "[GetTransaction] - Currency '{0}' not found.", request.CurrencyId);
					return null;
				}

				try
				{
					//var wallet = new WalletConnector("127.0.0.1", 33113, "sa_ddam213.3", "213bit");
					var wallet = new WalletConnector(currency.WalletHost, currency.WalletPort, currency.WalletUser, currency.WalletPass);
					return new GetWalletTransactionResponse { TransactionData = await wallet.GetTransactionRawAsync(request.TxId.Trim()) };
				}
				catch (Exception ex)
				{
					Log.Exception("[GetTransaction] - An exception occured during GetTransaction, CurrencyId: {0}.", ex, request.CurrencyId);
				}
				return null;
			}
		}

		public async Task<GetWalletBlockResponse> GetBlock(GetWalletBlockRequest request)
		{
			using (var currencyRepo = new Repository<Currency>())
			{
				var currency = await currencyRepo.GetOrDefaultAsync(x => x.Id == request.CurrencyId);
				if (currency == null)
				{
					Log.Message(LogLevel.Error, "[GetTransaction] - Currency '{0}' not found.", request.CurrencyId);
					return null;
				}

				try
				{
					// var wallet = new WalletConnector("127.0.0.1", 33113, "sa_ddam213.3", "213bit");
					var wallet = new WalletConnector(currency.WalletHost, currency.WalletPort, currency.WalletUser, currency.WalletPass);
					var blockHash = request.BlockHash;
					if (string.IsNullOrEmpty(blockHash))
					{
						blockHash = await wallet.GetBlockHashAsync(request.BlockHeight);
					}
					return new GetWalletBlockResponse { BlockData = await wallet.GetBlockAsync(blockHash.Trim()) };
				}
				catch (Exception ex)
				{
					Log.Exception("[GetBlock] - An exception occured during GetBlock, CurrencyId: {0}.", ex, request.CurrencyId);
				}
				return null;
			}
		}

		public async Task<GetWalletInfoResponse> GetInfo(int currencyId)
		{
			using (var currencyRepo = new Repository<Currency>())
			{
				var currency = await currencyRepo.GetOrDefaultAsync(x => x.Id == currencyId);
				if (currency == null)
				{
					Log.Message(LogLevel.Error, "[GetInfo] - Currency '{0}' not found.", currencyId);
					return null;
				}

				try
				{
					//var wallet = new WalletConnector("127.0.0.1", 33113, "sa_ddam213.3", "213bit");
					var wallet = new WalletConnector(currency.WalletHost, currency.WalletPort, currency.WalletUser, currency.WalletPass);
					var info = await wallet.GetInfoAsync();
					var miningInfo = await wallet.GetMiningInfoAsync();
					return new GetWalletInfoResponse
					{
						InfoData = new GetInfoData
						{
							Blocks = info.Blocks,
							Connections = info.Connections,
							Difficulty = info.Difficulty,
							Hashrate = miningInfo.NetworkHashrate
						},
						PeerInfo = new List<PeerInfo>(await wallet.GetPeerInfoAsync())
					};
				}
				catch (Exception ex)
				{
					Log.Exception("[GetInfo] - An exception occured during GetInfo, CurrencyId: {0}.", ex, currencyId);
				}
				return null;
			}
		}


		public async Task<GetWalletFeeResponse> GetWalletFee(int currencyId, decimal amount)
		{
			try
			{
				using (var currencyRepo = new Repository<Currency>())
				{
					var currency = await currencyRepo.GetOrDefaultAsync(x => x.Id == currencyId);
					if (currency == null)
					{
						return null;
					}

					//var wallet = new WalletConnector("127.0.0.1", 33113, "sa_ddam213.3", "213bit");
					var wallet = new WalletConnector(currency.WalletHost, currency.WalletPort, currency.WalletUser, currency.WalletPass);
					if (wallet == null)
					{
						Log.Message(LogLevel.Error, "[CreateAddress] - Wallet '{0}' not found.", currencyId);
						return null;
					}

					var fee = await wallet.GetTxFeeAsync(amount);
					return new GetWalletFeeResponse
					{
						Fee = fee
					};
				}
			}
			catch (Exception ex)
			{
				Log.Exception("[GetWalletFee] - An exception occured during GetWalletFee, CurrencyId: {0}.", ex, currencyId);
			}
			return null;
		}
	}
}
