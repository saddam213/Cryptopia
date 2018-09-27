namespace Cryptopia.WalletAPI.DataObjects
{
	/// <summary>
	/// The transaction type
	/// </summary>
	public enum TransactionDataType
    {
        /// <summary>
        /// All
        /// </summary>
        All,
        /// <summary>
        /// deposit
        /// </summary>
        Deposit,
        /// <summary>
        /// withdraw
        /// </summary>
        Withdraw,
        /// <summary>
        /// transfer
        /// </summary>
        Transfer,

        /// <summary>
        /// Mined
        /// </summary>
        Mined,

        /// <summary>
        /// The immature
        /// </summary>
        Immature,

        Orphan,
		Savings
    }
}