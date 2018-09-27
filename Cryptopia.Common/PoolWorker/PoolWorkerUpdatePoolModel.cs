using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cryptopia.Common.Pool;
using System.ComponentModel.DataAnnotations;
using Cryptopia.Enums;

namespace Cryptopia.Common.PoolWorker
{
	public class PoolWorkerUpdatePoolModel
	{
		public string Name { get; set; }

		[Required]
		public string TargetPool { get; set; }

		public List<PoolModel> Pools { get; set; }
		public AlgoType AlgoType { get; set; }
		public int Id { get; set; }
	}
}
