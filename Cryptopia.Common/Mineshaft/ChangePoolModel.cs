using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cryptopia.Common.Pool;
using Cryptopia.Enums;

namespace Cryptopia.Common.Mineshaft
{
	public class ChangePoolModel
	{
		public List<PoolWorkerModel> Workers { get; set; }

		public List<int> SelectedWorkers { get; set; } = new List<int>();
		public int PoolId { get; set; }
		public string PoolName { get; set; }
		public string PoolSymbol { get; set; }
		public AlgoType AlgoType { get; set; }
		public bool AllWorkers { get; set; }
	}
}
