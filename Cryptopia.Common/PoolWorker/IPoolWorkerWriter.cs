using Cryptopia.Common.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cryptopia.Infrastructure.Common.Results;

namespace Cryptopia.Common.PoolWorker
{
	public interface IPoolWorkerWriter
	{
		Task<IWriterResult> CreateWorker(string userId, PoolWorkerCreateModel model);
		Task<IWriterResult> DeleteWorker(string userId, int workerId);
		Task<IWriterResult> UpdateWorker(string userId, PoolWorkerUpdateModel model);
		Task<IWriterResult> UpdateWorkerPool(string userId, PoolWorkerUpdatePoolModel model);
		Task<IWriterResult> AdminUpdateWorker(AdminPoolWorkerUpdateModel model);
		Task<IWriterResult> AdminUpdateWorkerPool(AdminUpdateWorkerPoolModel model);
	}
}
