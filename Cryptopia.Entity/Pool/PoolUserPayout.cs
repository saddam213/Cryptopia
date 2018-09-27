using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Cryptopia.Enums;
using System;

namespace Cryptopia.Entity
{
	public class PoolUserPayout
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		public int PoolId { get; set; }

		[Index("IX_UserPayout", 1, IsUnique = true)]
		public Guid UserId { get; set; }

		[Index("IX_UserPayout", 2, IsUnique = true)]
		public int BlockId { get; set; }

		public decimal Amount { get; set; }

		public double Shares { get; set; }

		public int TransferId { get; set; }

		public PoolPayoutStatus Status { get; set; }

		[ForeignKey("PoolId")]
		public virtual Pool Pool { get; set; }

		[ForeignKey("UserId")]
		public virtual PoolUser User { get; set; }

		[ForeignKey("BlockId")]
		public virtual PoolBlock Block { get; set; }
	}
}