using System.Threading.Tasks;
using Cryptopia.Common.Trade;
using Cryptopia.Infrastructure.Common.Results;

namespace Cryptopia.Common.Withdraw
{
	public interface IWithdrawWriter
	{
		Task<IWriterResult> CancelWithdraw(string userId, CancelWithdrawModel model);
		Task<IWriterResult> ConfirmWithdraw(string userId, ConfirmWithdrawModel model);
		Task<IWriterResult<int>> CreateWithdraw(string userId, CreateWithdrawModel model);
		Task<IWriterResult> UpdateTwoFactorToken(string userId, UpdateTwoFactorTokenModel model);
	}
}