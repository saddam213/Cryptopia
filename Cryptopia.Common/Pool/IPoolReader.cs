using System.Threading.Tasks;
using Cryptopia.Infrastructure.Common.DataTables;
using System.Collections.Generic;
using Cryptopia.Enums;

namespace Cryptopia.Common.Pool
{
	public interface IPoolReader
	{
		Task<List<PoolModel>> GetPools();
		Task<PoolSettingsModel> GetPoolSettings();
		Task<List<PoolConnectionModel>> GetPoolConnections();
		Task<DataTablesResponse> GetPoolConnections(DataTablesModel model);
		Task<DataTablesResponse> AdminGetPools(string userId, DataTablesModel model);
		Task<AdminUpdatePoolModel> AdminGetPool(int poolId);
		Task<PoolConnectionModel> GetPoolConnection(AlgoType algoType);
		Task<PoolModel> GetPool(int poolId);
		Task<DataTablesResponse> GetBlocks(DataTablesModel model, int poolId);
		Task<DataTablesResponse> GetPayouts(string userId, DataTablesModel model, int poolId);
		Task<DataTablesResponse> GetPayouts(string userId, DataTablesModel model);
		Task<DataTablesResponse> AdminGetPayouts(string userId, DataTablesModel model);
	}
}