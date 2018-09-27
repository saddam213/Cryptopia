using System.Threading.Tasks;
using Cryptopia.Infrastructure.Common.DataTables;

namespace Cryptopia.Common.Withdraw
{
	public interface IWithdrawReader
	{
		Task<WithdrawModel> GetWithdrawal(string userId, int withdrawId);
		Task<DataTablesResponse> GetWithdrawals(string userId, DataTablesModel model);
	}
}