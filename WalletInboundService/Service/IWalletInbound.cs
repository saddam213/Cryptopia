using Cryptopia.InboundService.DataObjects;
using System;
using System.ServiceModel;
using System.Threading.Tasks;

namespace Cryptopia.InboundService
{
	[ServiceContract]
    public interface IWalletInbound
    {
        /// <summary>
        /// Creates an address for the specified wallet with the userid.
        /// </summary>
        /// <param name="walletId">The wallet id.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>the new addres created in the wallet</returns>
        [OperationContract]
        Task<string[]> CreateAddress(int walletId, Guid userId);

        /// <summary>
        /// Validates the address against the wallet.
        /// </summary>
        /// <param name="walletId">The wallet id.</param>
        /// <param name="address">The address.</param>
        /// <returns>true if the address is a valid address for the wallet, otherwise false</returns>
        [OperationContract]
        Task<bool> ValidateAddress(int walletId, string address);

        /// <summary>
        /// Gets the transaction.
        /// </summary>
        /// <param name="request">The request.</param>
        [OperationContract]
        Task<GetWalletTransactionResponse> GetTransaction(GetWalletTransactionRequest request);

        /// <summary>
        /// Gets the information about the currency.
        /// </summary>
        /// <param name="currencyId">The currency identifier.</param>
        [OperationContract]
        Task<GetWalletInfoResponse> GetInfo(int currencyId);

        /// <summary>
        /// Gets the block.
        /// </summary>
        /// <param name="request">The request.</param>
        [OperationContract]
        Task<GetWalletBlockResponse> GetBlock(GetWalletBlockRequest request);

		[OperationContract]
		Task<GetWalletFeeResponse> GetWalletFee(int currencyId, decimal amount);
    }


}
