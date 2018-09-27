using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Cryptopia.Enums;
using System;

namespace Cryptopia.Entity
{
	public class Pool
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		public int CurrencyId { get; set; }

		public AlgoType AlgoType { get; set; }

		[MaxLength(10)]
		public string Symbol { get; set; }

		[MaxLength(50)]
		public string Name { get; set; }

		[MaxLength(10)]
		public string TablePrefix { get; set; }

		public int BlockTime { get; set; }

		[MaxLength(128)]
		public string WalletUser { get; set; }

		[MaxLength(128)]
		public string WalletPass { get; set; }

		[MaxLength(128)]
		public string WalletHost { get; set; }

		public int WalletPort { get; set; }

		public decimal WalletFee { get; set; }

		public int Fee { get; set; }

		[MaxLength(4000)]
		public string SpecialInstructions { get; set; }

		public PoolStatus Status { get; set; }

		[MaxLength(1024)]
		public string StatusMessage { get; set; }

		[MaxLength(4000)]
		public string ExtraData { get; set; }

		public int BlockRefreshInterval { get; set; }
	
		public bool IsForkCheckDisabled { get; set; }

		public DateTime FeaturedExpires { get; set; }

		public DateTime Expires { get; set; }

		public bool IsEnabled { get; set; }

		public virtual PoolStatistics Statistics { get; set; }

		public virtual ICollection<PoolBlock> Blocks { get; set; }

		public virtual ICollection<PoolUserPayout> UserPayouts { get; set; }

		public virtual ICollection<PoolUserStatistics> UserStatistics { get; set; }
	}
}