using System.Threading.Tasks;

namespace Cryptopia.Common.Trade
{
	public interface ITradeService
	{
		Task<CreateTradeResponseModel> CreateTrade(string userId, CreateTradeModel model, bool isApi = false);
		Task<CreateTransferResponseModel> CreateTransfer(string userId, CreateTransferModel model, bool isApi = false);
		Task<CreateTipResponseModel> CreateTip(string userId, CreateTipModel model, bool isApi = false);
		Task<CancelTradeResponseModel> CancelTrade(string userId, CancelTradeModel model, bool isApi = false);
		Task<CreateLottoResponseModel> CreateLotto(string userId, CreateLottoModel model, bool isApi = false);
		Task<CreateWithdrawResponseModel> CreateWithdraw(string userId, CreateWithdrawModel model, bool isApi = false);
	}
}
