namespace Cryptopia.WalletAPI
{
    /// <summary>
    /// The valid wallet functions
    /// </summary>
	public enum WalletFunctionType
	{
		/// <summary>
		/// Validates an address
		/// </summary>
		ValidateAddress,

		/// <summary>
		/// Gets a new address
		/// </summary>
		GetNewAddress,

		/// <summary>
		/// Lists all the transactions since a certain block
		/// </summary>
		ListSinceBlock,

		/// <summary>
		/// Gets wallet info
		/// </summary>
		GetInfo,
		/// <summary>
		/// Gets balance
		/// </summary>
		GetBalance,

		/// <summary>
		/// Send coins from an account to an address
		/// </summary>
		SendToAddress,

		/// <summary>
		/// Get account address
		/// </summary>
		GetAccountAddress,

		/// <summary>
		/// Stop the wallet
		/// </summary>
		Stop,

		/// <summary>
		/// Gets the private key for the specified address
		/// </summary>
		DumpPrivKey,
		GetMiningInfo,
		Getblock,
		GetBlockCount,
		GetBlockHash,
		GetRawTransaction,
		GetPeerInfo,
		GetTransaction,
		GetTxFee,


		// Proxtopia
		GetDeposits
	}
}