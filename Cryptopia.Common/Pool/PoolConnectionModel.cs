using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cryptopia.Enums;

namespace Cryptopia.Common.Pool
{
	public class PoolConnectionModel
	{
		public AlgoType AlgoType { get; set; }
		public double DefaultDiff { get; set; }
		public string DefaultPool { get; set; }
		public string FixedDiffSummary { get; set; }
		public string Host { get; set; }
		public int Id { get; set; }
		public string Name { get; set; }
		public int Port { get; set; }
		public string VarDiffHighSummary { get; set; }
		public string VarDiffLowSummary { get; set; }
		public string VarDiffMediumSummary { get; set; }
	}
}
