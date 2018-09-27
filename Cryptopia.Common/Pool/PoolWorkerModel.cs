using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cryptopia.Enums;

namespace Cryptopia.Common.Pool
{
	public class PoolWorkerModel
	{
		public AlgoType AlgoType { get; set; }
		public string Password { get; set; }
		public double Difficulty { get; set; }
		public double Hashrate { get; set; }
		public int Id { get; set; }
		public bool IsActive { get; set; }
		public bool IsAutoSwitch { get; set; }
		public string Name { get; set; }
		public double TargetDifficulty { get; set; }
		public string TargetPool { get; set; }
	}
}
