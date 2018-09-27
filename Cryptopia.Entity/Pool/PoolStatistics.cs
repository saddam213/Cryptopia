using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cryptopia.Entity
{
	public class PoolStatistics
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		public int PoolId { get; set; }

		public double Hashrate { get; set; }

		public double NetworkHashrate { get; set; }

		public double NetworkDifficulty { get; set; }

		public double BlockProgress { get; set; }

		public decimal BlockReward { get; set; }

		public int CurrentBlock { get; set; }

		public int LastPoolBlock { get; set; }

		public DateTime? LastBlockTime { get; set; }

		public int EstimatedTime { get; set; }

		public double EstimatedShares { get; set; }

		public double ValidShares { get; set; }

		public double InvalidShares { get; set; }

		public decimal Profitability { get; set; }

		public int LastPayoutShareId { get; set; }

		public int Connections { get; set; }

		public decimal Balance { get; set; }

		[ForeignKey("PoolId")]
		public virtual Pool Pool { get; set; }
	}
}