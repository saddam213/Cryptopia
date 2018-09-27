using System;
using Cryptopia.Enums;

namespace Cryptopia.Common.Currency
{
	public class CurrencySummaryModel
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
		public DateTime LastUpdated { get; set; }
		public decimal PoolFee { get; set; }
		public decimal TradeFee { get; set; }
		public decimal WithdrawFee { get; set; }
		public WithdrawFeeType WithdrawFeeType { get; set; }
		public decimal WithdrawMin { get; set; }
		public decimal WithdrawMax { get; set; }
		public decimal TipMin { get; set; }
		public decimal MinBaseTrade { get; set; }
		public int MinConfirmations { get; set; }

		public CurrencyStatus Status { get; set; }
		public string StatusMessage { get; set; }
		public DateTime FeaturedExpires { get; set; }
		public DateTime TippingExpires { get; set; }
		public DateTime RewardsExpires { get; set; }
		public int CurrentBlock { get; set; }
		public double StarRating { get; set; }
		public int TotalRating { get; set; }
		public int MaxRating { get; set; }
		public CurrencyListingStatus ListingStatus { get; set; }
	}
}
