using System;
using System.ComponentModel.DataAnnotations;
using Cryptopia.Enums;

namespace Cryptopia.Entity
{
	public class CurrencyInfo
	{
		[Key]
		public int Id { get; set; }

		public AlgoType AlgoType { get; set; }
		public NetworkType NetworkType { get; set; }
		public double StarRating { get; set; }
		public int TotalRating { get; set; }
		public int MaxRating { get; set; }

		[MaxLength(4000)]
		public string Description { get; set; }

		[MaxLength(256)]
		public string Website { get; set; }

		[MaxLength(256)]
		public string Source { get; set; }

		[MaxLength(256)]
		public string BlockExplorer { get; set; }

		[MaxLength(256)]
		public string LaunchForum { get; set; }

		[MaxLength(256)]
		public string CryptopiaForum { get; set; }
		public int BlockTime { get; set; }
		public decimal BlockReward { get; set; }
		public decimal TotalCoin { get; set; }
		public decimal PosRate { get; set; }
		public int MinStakeAge { get; set; }
		public int MaxStakeAge { get; set; }
		public int DiffRetarget { get; set; }


		public decimal TotalPremine { get; set; }
		public bool WalletWindows { get; set; }
		public bool WalletLinux { get; set; }
		public bool WalletMac { get; set; }
		public bool WalletMobile { get; set; }
		public bool WalletWeb { get; set; }

		public DateTime LastUpdated { get; set; }

		public virtual Currency Currency { get; set; }
	
	}
}