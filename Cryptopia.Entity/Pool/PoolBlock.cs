using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Cryptopia.Enums;
using System;

namespace Cryptopia.Entity
{
	public class PoolBlock
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		[Index("IX_BlockHeight", 1, IsUnique = true)]
		public int PoolId { get; set; }

		[Index("IX_BlockHeight", 2, IsUnique = true)]
		public int Height { get; set; }

		public Guid UserId { get; set; }

		[MaxLength(256)]
		public string Hash { get; set; }

		[MaxLength(256)]
		public string TxId { get; set; }

		public decimal Amount { get; set; }

		public int Confirmations { get; set; }

		public double Difficulty { get; set; }

		public double Shares { get; set; }

		public double EstimatedShares { get; set; }

		public double Luck { get; set; }

		public PoolBlockStatus Status { get; set; }

		public DateTime Timestamp { get; set; }

		public bool IsProcessed { get; set; }

		[ForeignKey("PoolId")]
		public virtual Pool Pool { get; set; }

		[ForeignKey("UserId")]
		public virtual PoolUser User { get; set; }

		public virtual ICollection<PoolUserPayout> Payouts { get; set; }
		
	}
}