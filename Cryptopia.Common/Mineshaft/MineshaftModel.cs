using Cryptopia.Common.Pool;
using Cryptopia.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptopia.Common.Mineshaft
{
	public class MineshaftModel
	{
		public PoolModel CurrentPool { get; set; }
		public List<PoolModel> Pools { get; set; }
		public List<AlgoType> Algos { get; set; }
		public AlgoType? BaseAlgo { get; set; }
	}

	public class MineshaftInfoModel
	{
		public AlgoType AlgoType { get; set; }
		public double BlockProgress { get; set; }
		public int BlocksFound { get; set; }
		public int CurrentBlock { get; set; }
		public double EstimatedShares { get; set; }
		public int EstimatedTime { get; set; }
		public double Hashrate { get; set; }
		public int Id { get; set; }
		public double InvalidShares { get; set; }
		public DateTime? LastBlockTime { get; set; }
		public int LastPoolBlock { get; set; }
		public double Luck { get; set; }
		public string Name { get; set; }
		public double NetworkDifficulty { get; set; }
		public double NetworkHashrate { get; set; }
		public PoolStatus Status { get; set; }
		public string StatusMessage { get; set; }
		public string StratumHost { get; set; }
		public int StratumPort { get; set; }
		public string Symbol { get; set; }
		public int UserCount { get; set; }
		public double ValidShares { get; set; }
		public DateTime Expires { get; set; }
		public decimal Profitability { get; set; }
	}


	public class MineshaftUserInfoModel
	{
		public decimal Confirmed { get; set; }
		public decimal Unconfirmed { get; set; }
		public double InvalidShares { get; set; }
		public double ValidShares { get; set; }
		public double Hashrate { get; set; }
		public int WorkerCount { get; set; }
		public int ActiveWorkerCount { get; set; }
	}

	public class MineshaftSummary
	{
		public MineshaftSummary()
		{
			AlgoType = null;
			Featured = new List<FeaturedPool>();
			TopPools= new List<MineshaftSummaryModel>();
			AlgoTypes = new List<AlgoTypeInfo>();
		}

		public int TotalPools { get; set; }
		public double TotalHashrate { get; set; }
		public List<FeaturedPool> Featured { get; set; }
		public List<AlgoTypeInfo> AlgoTypes { get; set; }
		public List<MineshaftSummaryModel> TopPools { get; set; }
		public AlgoType? AlgoType { get; set; }
	}

	public class AlgoTypeInfo
	{
		public string Name { get; set; }
		public AlgoType? AlgoType { get; set; }
		public string TopPoolSymbol { get; set; }
		public double TotalHashrate { get; set; }
	}
}
