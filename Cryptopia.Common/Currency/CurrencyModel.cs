using Cryptopia.Enums;
using System;

namespace Cryptopia.Common.Currency
{
	public class CurrencyModel
	{
		public int CurrencyId { get; set; }
		public string Symbol { get; set; }
		public string Name { get; set; }
		public AlgoType AlgoType { get; set; }
		public decimal PoolFee { get; set; }
		public decimal TradeFee { get; set; }
		public decimal WithdrawFee { get; set; }
		public decimal WithdrawMin { get; set; }
		public decimal WithdrawMax { get; set; }
		public decimal TipMin { get; set; }
		public CurrencyStatus Status { get; set; }
		public CurrencyListingStatus ListingStatus { get; set; }
		public string StatusMessage { get; set; }
		public int ForumId { get; set; }
		public decimal MinBaseTrade { get; set; }
		public int BlockTime { get; set; }
		public string Summary { get; set; }
		public string Version { get; set; }
		public int Connections { get; set; }
		public string Errors { get; set; }
		public NetworkType NetworkType { get; set; }
		public int Block { get; set; }
		public int MinConfirmations { get; set; }
		public WithdrawFeeType WithdrawFeeType { get; set; }
		public string BaseAddress { get; set; }
		public CurrencyType Type { get; set; }
		public DateTime TippingExpires { get; set; }
		public DateTime FeaturedExpires { get; set; }
		public DateTime RewardsExpires { get; set; }
		public string Website { get; set; }
		public int Rank { get; set; }

		public string QrFormat { get; set; }
		public string DepositInstructions { get; set; }
		public string DepositMessage { get; set; }
		public AlertType DepositMessageType { get; set; }
		public string WithdrawInstructions { get; set; }
		public string WithdrawMessage { get; set; }
		public AlertType WithdrawMessageType { get; set; }
		public AddressType AddressType { get; set; }

		public string DisplayName
		{
			get { return $"{Name}({Symbol})"; }
		}

		public int CurrencyDecimals { get; set; }
	}
}