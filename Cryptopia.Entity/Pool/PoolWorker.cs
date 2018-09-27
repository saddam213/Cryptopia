using Cryptopia.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cryptopia.Entity
{
	public class PoolWorker
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		public Guid UserId { get; set; }

		[MaxLength(50)]
		[Index("IX_WorkerAlgo", 1, IsUnique = true)]
		public string Name { get; set; }

		[Index("IX_WorkerAlgo", 2, IsUnique = true)]
		public AlgoType AlgoType { get; set; }

		[MaxLength(50)]
		public string Password { get; set; }

		public double Difficulty { get; set; }

		public double Hashrate { get; set; }

		public double TargetDifficulty { get; set; }
		
		[MaxLength(10)]
		public string TargetPool { get; set; }

		public bool IsActive { get; set; }

		public bool IsAutoSwitch { get; set; }

		public bool IsEnabled { get; set; }

		[ForeignKey("UserId")]
		public virtual PoolUser User { get; set; }
	}
}