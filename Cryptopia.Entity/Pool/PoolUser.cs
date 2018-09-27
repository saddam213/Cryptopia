using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cryptopia.Entity
{
	public class PoolUser
	{
		[Key]
		public Guid Id { get; set; }

		[MaxLength(128)]
		public string UserName { get; set; }

		[MaxLength(128)]
		public string MiningHandle { get; set; }

		public bool DisableNotifications { get; set; }

		public bool IsEnabled { get; set; }

		public virtual ICollection<PoolWorker> Workers { get; set; }
		public virtual ICollection<PoolUserPayout> Payouts { get; set; }
		public virtual ICollection<PoolUserStatistics> Statistics { get; set; }

	}
}