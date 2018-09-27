using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cryptopia.Enums;

namespace Cryptopia.Common.Mineshaft
{
	public class GettingStartedModel
	{
		public AlgoType AlgoType { get; set; }
		public int PoolId { get; set; }
		public string PoolName { get; set; }
		public string PoolSymbol { get; set; }
		public int Port { get; set; }
		public string StratumUrl { get; set; }
	}
}
