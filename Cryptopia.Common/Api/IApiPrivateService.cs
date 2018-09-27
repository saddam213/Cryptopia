using Cryptopia.Common.Trade;
using System.Threading.Tasks;

namespace Cryptopia.Common.Api
{
	public interface IApiPrivateService
	{
		Task<CreateTradeResponseModel> SubmitUserTrade(ApiSubmitUserTradeRequest request);
		Task<CreateTransferResponseModel> SubmitUserTransfer(ApiSubmitUserTransferRequest request);
		Task<CancelTradeResponseModel> CancelUserTrade(ApiCancelUserTradeRequest request);
		Task<ApiUserBalanceResponse> GetUserBalance(ApiUserBalanceRequest request);
		Task<ApiUserOpenOrdersResponse> GetUserOpenOrders(ApiUserOpenOrdersRequest request);
		Task<ApiUserTradeHistoryResponse> GetUserTradeHistory(ApiUserTradeHistoryRequest request);
		Task<ApiUserTransactionsResponse> GetUserTransactions(ApiUserTransactionsRequest request);
		Task<ApiUserDepositAddressResponse> GetUserDepositAddress(ApiUserDepositAddressRequest request);
		Task<CreateTipResponseModel> SubmitUserTip(ApiSubmitUserTipRequest request);
		Task<ApiSubmitUserWithdrawResponse> SubmitUserWithdraw(ApiSubmitUserWithdrawRequest request);
	}
}
