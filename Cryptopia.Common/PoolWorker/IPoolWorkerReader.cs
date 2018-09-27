using Cryptopia.Infrastructure.Common.DataTables;
using Cryptopia.Common.Pool;
using Cryptopia.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptopia.Common.PoolWorker
{
	public interface IPoolWorkerReader
	{
		Task<PoolWorkerModel> GetWorker(string userId, int workerId);
		Task<List<PoolWorkerModel>> GetWorkers(string userId, AlgoType algoType);
		Task<DataTablesResponse> GetWorkers(string userId, DataTablesModel model);
		Task<DataTablesResponse> GetWorkers(int poolId, DataTablesModel model);

		Task<PoolWorkerModel> AdminGetWorker(string userId, int workerId);
		Task<DataTablesResponse> AdminGetWorkers(string userId, DataTablesModel model);
	}
}
