using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cryptopia.Enums;

namespace Cryptopia.Common.Pool
{
	public class PoolModel
	{
		public AlgoType AlgoType { get; set; }
		public double DefaultDiff { get; set; }
		public string FixedDiffSummary { get; set; }
		public double Hashrate { get; set; }
		public double NetworkHashrate { get; set; }
		public double NetworkDifficulty { get; set; }
		public int Id { get; set; }
		public DateTime Expires { get; set; }
		public DateTime FeaturedExpires { get; set; }
		public string Name { get; set; }
		public PoolStatus Status { get; set; }
		public string StatusMessage { get; set; }
		public string Symbol { get; set; }
		public string TablePrefix { get; set; }
		public string VarDiffHighSummary { get; set; }
		public string VarDiffLowSummary { get; set; }
		public string VarDiffMediumSummary { get; set; }
		public int CurrencyId { get; set; }
	}
}
