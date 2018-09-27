using Cryptopia.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace Cryptopia.Entity
{
	public class Currency
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		[MaxLength(128)]
		public string Name { get; set; }

		[MaxLength(10)]
		public string Symbol { get; set; }
		
		public decimal Balance { get; set; }

		[MaxLength(128)]
		public string WalletUser { get; set; }

		[MaxLength(128)]
		public string WalletPass { get; set; }

		public int WalletPort { get; set; }

		[MaxLength(128)]
		public string WalletHost { get; set; }

		public decimal TradeFee { get; set; }
		public decimal MinTradeAmount { get; set; }
		public decimal MaxTradeAmount { get; set; }
		public decimal MinBaseTrade { get; set; }
		public decimal TxFee { get; set; }
		public decimal PoolFee { get; set; }
		public decimal WithdrawFee { get; set; }
		public WithdrawFeeType WithdrawFeeType { get; set; }
		public decimal MinWithdraw { get; set; }
		public decimal MaxWithdraw { get; set; }
		public decimal MinTip { get; set; }
		public int MinConfirmations { get; set; }
		public int Rank { get; set; }
		public CurrencyStatus Status { get; set; }

		public CurrencyListingStatus ListingStatus { get; set; }

		[MaxLength(4000)]
		public string StatusMessage { get; set; }

		[MaxLength(256)]
		public string LastBlockHash { get; set; }

		[MaxLength(256)]
		public string LastWithdrawBlockHash { get; set; }
		public int Block { get; set; }
				
		[MaxLength(128)]
		public string Version { get; set; }

		public int Connections { get; set; }

		[MaxLength(4000)]
		public string Errors { get; set; }

		public bool IsEnabled { get; set; }

		[MaxLength(128)]
		public string BaseAddress { get; set; }

		public CurrencyType Type { get; set; }

		public InterfaceType InterfaceType { get; set; }



		public DateTime FeaturedExpires { get; set; }
		public DateTime TippingExpires { get; set; }
		public DateTime RewardsExpires { get; set; }

		

		public virtual CurrencyInfo Info { get; set; }
		public virtual CurrencySettings Settings { get; set; }




		// Columns to remove from DB after context change
		//public string Summary { get; set; }
		//public int ForumId { get; set; }
		//public int BlockTime { get; set; }
		//public decimal MinFaucet { get; set; }
		//public decimal FaucetPercent { get; set; }
		//public bool IsFaucetEnabled { get; set; }
		//public AlgoType AlgoType { get; set; }
		//public NetworkType NetworkType { get; set; }
	}
}