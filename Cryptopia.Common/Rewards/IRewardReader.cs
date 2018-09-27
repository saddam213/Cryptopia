using Cryptopia.Infrastructure.Common.DataTables;
using System.Threading.Tasks;

namespace Cryptopia.Common.Rewards
{
	public interface IRewardReader
	{
		Task<DataTablesResponse> GetHistory(string userId, DataTablesModel model);
		Task<DataTablesResponse> GetBalances(string userId, DataTablesModel model);
		Task<DataTablesResponse> GetStatistics(string userId, DataTablesModel model);
	}
}
