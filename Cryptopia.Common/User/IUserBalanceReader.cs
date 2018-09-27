using System.Threading.Tasks;
using Cryptopia.Infrastructure.Common.DataTables;

namespace Cryptopia.Common.User
{
	public interface IUserBalanceReader
	{
		Task<UserBalanceItemModel> GetBalance(string userId, int currencyId);
		Task<UserBalanceModel> GetBalances(string userId, bool calculateEstimate);
	}
}