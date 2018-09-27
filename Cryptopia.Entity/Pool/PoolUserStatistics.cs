using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cryptopia.Entity
{
	public class PoolUserStatistics
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		[Index("IX_UserPool", 1, IsUnique = true)]
		public Guid UserId { get; set; }

		[Index("IX_UserPool", 2, IsUnique = true)]
		public int PoolId { get; set; }

		public double Hashrate { get; set; }

		public double ValidShares { get; set; }

		public double InvalidShares { get; set; }


		[ForeignKey("UserId")]
		public virtual PoolUser User { get; set; }

		[ForeignKey("PoolId")]
		public virtual Pool Pool { get; set; }
	}
}