using Cryptopia.Common.Pool;
using Cryptopia.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptopia.Common.PoolWorker
{
	public class AdminUpdateWorkerPoolModel
	{
		public int PoolId { get; set; }
		public AlgoType AlgoType { get; set; }
		public List<PoolModel> Pools { get; set; }
	}
}
