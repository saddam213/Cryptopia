using Cryptopia.Enums;

namespace Cryptopia.Common.Currency
{
	public class UpdateCurrencyInfoModel
	{
		public int Id { get; set; }
		public string Symbol { get; set; }
		public string Name { get; set; }
		public string Summary { get; set; }

		public AlgoType AlgoType { get; set; }
		public NetworkType NetworkType { get; set; }
		public int BlockTime { get; set; }
		public decimal BlockReward { get; set; }
		public decimal TotalCoin { get; set; }
		public decimal PosRate { get; set; }
		public int MinStakeAge { get; set; }
		public int MaxStakeAge { get; set; }
		public int DiffRetarget { get; set; }

		public string Website { get; set; }
		public string Source { get; set; }
		public string BlockExplorer { get; set; }
		public string LaunchForum { get; set; }
		public string CryptopiaForum { get; set; }

		public decimal TotalPremine { get; set; }
		public bool WalletWindows { get; set; }
		public bool WalletLinux { get; set; }
		public bool WalletMac { get; set; }
		public bool WalletMobile { get; set; }
		public bool WalletWeb { get; set; }
	
	}
}
