using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cryptopia.WalletAPI.DataObjects;
using Cryptopia.WalletAPI.Helpers;
using Newtonsoft.Json;

namespace Cryptopia.WalletAPI.Base
{
	/// <summary>
	/// Class to hadle RPC calls to a wallet
	/// </summary>
	public class WalletConnector : Connector
	{
		public WalletConnector(string ip, int port, string username, string password, int timeout = 60000)
			: base(ip, port, username, password, timeout)
		{
		}

		/// <summary>
		/// Generates a new coin address.
		/// </summary>
		/// <param name="accountId">The id to identify the address in the wallet.</param>
		/// <returns>a new address hash if successful, otherwise false</returns>
		public string GenerateAddress(string accountId, bool allowMultiple = false)
		{
			if (!allowMultiple)
			{
				var existing = ExecuteWalletFunction<string>(WalletFunctionType.GetAccountAddress, accountId);
				if (!string.IsNullOrEmpty(existing))
				{
					return existing;
				}
			}
			var rawTransaction = ExecuteWalletFunction<string>(WalletFunctionType.GetNewAddress, accountId);
			return rawTransaction;
		}

		/// <summary>
		/// Generates the address async.
		/// </summary>
		/// <param name="accountId">The account id.</param>
		/// <returns></returns>
		public Task<string> GenerateAddressAsync(string accountId)
		{
			return Task.Factory.StartNew(() => GenerateAddress(accountId));
		}

		/// <summary>
		/// Validates the address.
		/// </summary>
		/// <param name="address">The address to validate.</param>
		/// <returns>true if the address hash is valid, otherwise false</returns>
		public bool ValidateAddress(string address)
		{
			var valid = ExecuteWalletFunction<ValidateAddressData>(WalletFunctionType.ValidateAddress, address);
			return valid != null && valid.IsValid;
		}

		/// <summary>
		/// Validates the address async.
		/// </summary>
		/// <param name="address">The address.</param>
		/// <returns></returns>
		public Task<bool> ValidateAddressAsync(string address)
		{
			return Task.Factory.StartNew(() => ValidateAddress(address));
		}

		/// <summary>
		/// Gets the transactions.
		/// </summary>
		/// <param name="lastblock"></param>
		/// <param name="transactionType">the type of transaction.</param>
		/// <returns>a List of Transactions</returns>
		public IEnumerable<TransactionData> GetTransactions(string lastblock = "",
			TransactionDataType transactionType = TransactionDataType.All)
		{
			var transactionData = string.IsNullOrEmpty(lastblock)
				? ExecuteWalletFunction<ListSinceBlockData>(WalletFunctionType.ListSinceBlock)
				: ExecuteWalletFunction<ListSinceBlockData>(WalletFunctionType.ListSinceBlock, lastblock);
			if (transactionData == null || transactionData.Transactions == null || !transactionData.Transactions.Any())
				return new List<TransactionData>();
			if (transactionType != TransactionDataType.All)
			{
				return transactionData.Transactions
					.Where(x => x.Category.Equals(transactionType.ToTransactionString()))
					.OrderBy(x => x.Time);
			}
			return transactionData.Transactions.OrderBy(x => x.Time);
		}

		/// <summary>
		/// Gets the transactions async.
		/// </summary>
		/// <param name="lastblock"></param>
		/// <param name="transactionType">Type of the transaction.</param>
		/// <returns></returns>
		public Task<IEnumerable<TransactionData>> GetTransactionsAsync(string lastblock = "",
			TransactionDataType transactionType = TransactionDataType.All)
		{
			return Task.Factory.StartNew<IEnumerable<TransactionData>>(() => GetTransactions(lastblock, transactionType));
		}

		public IEnumerable<TransactionData> GetDeposits(string lastblock = "")
		{
			var transactionData = string.IsNullOrEmpty(lastblock)
				? ExecuteWalletFunction<ListSinceBlockData>(WalletFunctionType.ListSinceBlock)
				: ExecuteWalletFunction<ListSinceBlockData>(WalletFunctionType.ListSinceBlock, lastblock);
			if (transactionData == null || transactionData.Transactions == null || !transactionData.Transactions.Any())
			{
				return new List<TransactionData>();
			}

			return transactionData.Transactions
				.Where(
					x =>
						x.Category.Equals(TransactionDataType.Deposit.ToTransactionString()) ||
						x.Category.Equals(TransactionDataType.Savings.ToTransactionString()))
				.OrderBy(x => x.Time);
		}

		public Task<IEnumerable<TransactionData>> GetDepositsAsync(string lastblock = "")
		{
			return Task.Factory.StartNew<IEnumerable<TransactionData>>(() => GetDeposits(lastblock));
		}



		public IEnumerable<TransactionData> GetDeposits(List<string> addresses, string lastblock = "")
		{
			var transactionData = ExecuteWalletFunction<ListSinceBlockData>(WalletFunctionType.GetDeposits, addresses, lastblock);
			if (transactionData == null || transactionData.Transactions == null || !transactionData.Transactions.Any())
			{
				return new List<TransactionData>();
			}

			return transactionData.Transactions.Where(x => x.Category.Equals(TransactionDataType.Deposit.ToTransactionString())).OrderBy(x => x.Time);
		}

		public Task<IEnumerable<TransactionData>> GetDepositsAsync(List<string> addresses, string lastblock = "")
		{
			return Task.Factory.StartNew<IEnumerable<TransactionData>>(() => GetDeposits(addresses, lastblock));
		}



		/// <summary>
		/// Gets the mined blocks.
		/// </summary>
		/// <param name="lastblock">The lastblock.</param>
		/// <returns></returns>
		public IEnumerable<TransactionData> GetMinedBlocks(string lastblock = "")
		{
			var transactionData = string.IsNullOrEmpty(lastblock)
				? ExecuteWalletFunction<ListSinceBlockData>(WalletFunctionType.ListSinceBlock)
				: ExecuteWalletFunction<ListSinceBlockData>(WalletFunctionType.ListSinceBlock, lastblock);
			if (transactionData == null || transactionData.Transactions == null || !transactionData.Transactions.Any())
			{
				return new List<TransactionData>();
			}

			return transactionData.Transactions
				.Where(x => x.Category.Equals("generate") || x.Category.Equals("immature") || x.Category.Equals("orphan"))
				.OrderBy(x => x.Time);
		}

		/// <summary>
		/// Gets the mined blocks.
		/// </summary>
		/// <param name="lastblock">The lastblock.</param>
		/// <returns></returns>
		public Task<IEnumerable<TransactionData>> GetMinedBlocksAsync(string lastblock = "")
		{
			return Task.Factory.StartNew<IEnumerable<TransactionData>>(() => GetMinedBlocks(lastblock));
		}

		/// <summary>
		/// Gets the information.
		/// </summary>
		/// <returns></returns>
		public GetInfoData GetInfo()
		{
			return GetInfoData();
		}

		/// <summary>
		/// Gets the information asynchronous.
		/// </summary>
		/// <returns></returns>
		public Task<GetInfoData> GetInfoAsync()
		{
			return Task.Factory.StartNew<GetInfoData>(() => GetInfo());
		}

		/// <summary>
		/// Gets the information.
		/// </summary>
		/// <returns></returns>
		public GetMiningInfoData GetMiningInfo()
		{
			return GetMiningInfoData();
		}

		/// <summary>
		/// Gets the information asynchronous.
		/// </summary>
		/// <returns></returns>
		public Task<GetMiningInfoData> GetMiningInfoAsync()
		{
			return Task.Factory.StartNew<GetMiningInfoData>(() => GetMiningInfo());
		}

		/// <summary>
		///  Gets the balance.
		/// </summary>
		public decimal GetBalance()
		{
			return ExecuteWalletFunction<decimal>(WalletFunctionType.GetBalance);
		}

		/// <summary>
		///  Gets the balance asynchronous.
		/// </summary>
		public Task<decimal> GetBalanceAsync()
		{
			return Task.Factory.StartNew<decimal>(GetBalance);
		}

		/// <summary>
		/// Sends coins from wallet to the specified address.
		/// </summary>
		/// <param name="userId">The user identifier.</param>
		/// <param name="address">The address.</param>
		/// <param name="amount">The amount.</param>
		/// <returns>the blockchain transaction created</returns>
		public WithdrawData SendToAddress(string address, decimal amount)
		{
			var transaction = ExecuteWalletFunction<string>(WalletFunctionType.SendToAddress, address, amount);
			return new WithdrawData { Txid = transaction };
		}

		/// <summary>
		/// Sends coins from wallet to the specified address asynchronously.
		/// </summary>
		/// <param name="userId">The user identifier.</param>
		/// <param name="address">The address.</param>
		/// <param name="amount">The amount.</param>
		/// <returns>the blockchain transaction created</returns>
		public Task<WithdrawData> SendToAddressAsync(string address, decimal amount)
		{
			return Task.Factory.StartNew<WithdrawData>(() => SendToAddress(address, amount));
		}

		/// <summary>
		/// Dumps the private key for an address.
		/// </summary>
		/// <param name="address">The address.</param>
		/// <returns>the private key for the specified address</returns>
		public string DumpPrivKey(string address)
		{
			return GetPrivateKey(address);
		}

		/// <summary>
		/// Dumps the private key asynchronous.
		/// </summary>
		/// <param name="address">The address.</param>
		/// <returns>the private key for the specified address</returns>
		public Task<string> DumpPrivKeyAsync(string address)
		{
			return Task.Factory.StartNew<string>(() => DumpPrivKey(address));
		}

		/// <summary>
		/// Gets the block.
		/// </summary>
		/// <param name="blockHash">The block hash.</param>
		public BlockData GetBlock(string blockHash)
		{
			var block = ExecuteWalletFunction<BlockData>(WalletFunctionType.Getblock, blockHash);
			return block;
		}

		/// <summary>
		/// Gets the block asynchronous.
		/// </summary>
		/// <param name="blockHash">The block hash.</param>
		public Task<BlockData> GetBlockAsync(string blockHash)
		{
			return Task.Factory.StartNew<BlockData>(() => GetBlock(blockHash));
		}

		/// <summary>
		/// Stops this wallet instance.
		/// </summary>
		/// <returns></returns>
		public bool Stop()
		{
			var result = ExecuteWalletFunction<string>(WalletFunctionType.Stop);
			return !string.IsNullOrEmpty(result);
		}

		/// <summary>
		/// Stops the wallet instance asynchronously.
		/// </summary>
		/// <returns></returns>
		public Task<bool> StopAsync()
		{
			return Task.Factory.StartNew<bool>(Stop);
		}


		public long GetBlockCount()
		{
			return ExecuteWalletFunction<long>(WalletFunctionType.GetBlockCount);
		}


		public string GetBlockHash(int blockHeight)
		{
			return ExecuteWalletFunction<string>(WalletFunctionType.GetBlockHash, blockHeight);
		}

		public Task<string> GetBlockHashAsync(int blockHeight)
		{
			return Task.Factory.StartNew<string>(() => GetBlockHash(blockHeight));
		}

		public TransactionRawData GetTransactionRaw(string txid)
		{
			return ExecuteWalletFunction<TransactionRawData>(WalletFunctionType.GetRawTransaction, txid, 1);
		}

		public Task<TransactionRawData> GetTransactionRawAsync(string txid)
		{
			return Task.Factory.StartNew<TransactionRawData>(() => GetTransactionRaw(txid));
		}

		public TransactionRawData GetTransaction(string txid)
		{
			return ExecuteWalletFunction<TransactionRawData>(WalletFunctionType.GetTransaction, txid);
		}

		public Task<TransactionRawData> GetTransactionAsync(string txid)
		{
			return Task.Factory.StartNew<TransactionRawData>(() => GetTransaction(txid));
		}

		public IEnumerable<PeerInfo> GetPeerInfo()
		{
			return ExecuteWalletFunction<IEnumerable<PeerInfo>>(WalletFunctionType.GetPeerInfo);
		}


		public Task<IEnumerable<PeerInfo>> GetPeerInfoAsync()
		{
			return Task.Factory.StartNew<IEnumerable<PeerInfo>>(GetPeerInfo);
		}


		public decimal GetTxFee(decimal amount)
		{
			return ExecuteWalletFunction<decimal>(WalletFunctionType.GetTxFee, amount);
		}

		public Task<decimal> GetTxFeeAsync(decimal amount)
		{
			return Task.Factory.StartNew<decimal>(() => GetTxFee(amount));
		}

		#region Private Methods

		/// <summary>
		/// Gets the mining information data.
		/// </summary>
		private GetMiningInfoData GetMiningInfoData()
		{
			var info = GetMiningInfoV1();
			if (info != null && info.Difficulty > 0)
			{
				return info;
			}

			var info2 = GetMiningInfoV2();
			if (info2 != null && info2.Difficulty != null && info2.Difficulty.Difficulty > 0)
			{
				return new GetMiningInfoData
				{
					Blocks = info2.Blocks,
					Difficulty = info2.Difficulty != null ? info2.Difficulty.Difficulty : 0,
					NetworkHashrate = (double)(info2.NetworkHashrate * 1000.0 * 1000.0)
				};
			}
			var info3 = GetMiningInfoV3();
			if (info3 != null && info3.Difficulty != null && info3.Difficulty.Difficulty > 0)
			{
				return new GetMiningInfoData
				{
					Blocks = info3.Blocks,
					Difficulty = info3.Difficulty != null ? info3.Difficulty.Difficulty : 0,
					NetworkHashrate = (double)(info3.NetworkHashrate * 1000.0 * 1000.0)
				};
			}

			var info4 = GetMiningInfoV4();
			if (info4 != null)
			{
				return new GetMiningInfoData
				{
					Blocks = info4.Blocks,
					Difficulty = info4.Difficulty,
					NetworkHashrate = info4.NetworkHashrate
				};
			}
			return new GetMiningInfoData();
		}

		private GetInfoData GetInfoData()
		{
			var info = GetInfoV1();
			if (info != null)
			{
				return info;
			}

			var posInfo = GetInfoV2();
			if (posInfo != null)
			{
				return new GetInfoData
				{
					Blocks = posInfo.Blocks,
					Difficulty = posInfo.Difficulty != null ? posInfo.Difficulty.Difficulty : 0,
					Connections = posInfo.Connections
				};
			}
			return new GetInfoData();
		}


		/// <summary>
		/// Gets the mining information v1 (normail coins).
		/// </summary>
		/// <returns></returns>
		private GetMiningInfoData GetMiningInfoV1()
		{
			try
			{
				return ExecuteWalletFunction<GetMiningInfoData>(WalletFunctionType.GetMiningInfo);
			}
			catch
			{
			}
			return null;
		}

		/// <summary>
		/// Gets the mining information v2 (pos cons).
		/// </summary>
		/// <returns></returns>
		private GetMiningInfoDataPOS GetMiningInfoV2()
		{
			try
			{
				return ExecuteWalletFunction<GetMiningInfoDataPOS>(WalletFunctionType.GetMiningInfo);
			}
			catch
			{
			}
			return null;
		}

		/// <summary>
		/// Gets the mining information v2 (pos cons).
		/// </summary>
		/// <returns></returns>
		private GetMiningInfoDataPOS2 GetMiningInfoV3()
		{
			try
			{
				return ExecuteWalletFunction<GetMiningInfoDataPOS2>(WalletFunctionType.GetMiningInfo);
			}
			catch
			{
			}
			return null;
		}

		/// <summary>
		/// Gets the mining information v2 (pos cons).
		/// </summary>
		/// <returns></returns>
		private GetMiningInfoDataPOS3 GetMiningInfoV4()
		{
			try
			{
				return ExecuteWalletFunction<GetMiningInfoDataPOS3>(WalletFunctionType.GetMiningInfo);
			}
			catch
			{
			}
			return null;
		}

		private GetInfoData GetInfoV1()
		{
			try
			{
				return ExecuteWalletFunction<GetInfoData>(WalletFunctionType.GetInfo);
			}
			catch
			{
			}
			return null;
		}


		/// <summary>
		/// Gets the mining information v2 (pos cons).
		/// </summary>
		/// <returns></returns>
		private GetInfoDataPOS GetInfoV2()
		{
			try
			{
				return ExecuteWalletFunction<GetInfoDataPOS>(WalletFunctionType.GetInfo);
			}
			catch
			{
			}
			return null;
		}

		/// <summary>
		/// Gets the private key or the address specified.
		/// </summary>
		/// <param name="address">The address.</param>
		private string GetPrivateKey(string address)
		{
			var rawPrivateKey = ExecuteWalletFunction<string>(WalletFunctionType.DumpPrivKey, address);
			if (rawPrivateKey.Contains('{'))
			{
				// Some wallets return a json object containing the private key
				var privateKeyObj = JsonConvert.DeserializeObject<PrivateKeyData>(rawPrivateKey);
				if (privateKeyObj != null)
				{
					return privateKeyObj.PrivateKey;
				}
			}
			return rawPrivateKey;
		}

		#endregion
	}
}