using Cryptopia.Common.Webservice;
using System.Data.Linq.Mapping;
using Cryptopia.Enums;
using System;

namespace Cryptopia.API.Objects
{
	[Table(Name = "dbo.Currency")]
	public class Currency : IIdentity<int>
	{
		[Column(AutoSync = AutoSync.OnInsert, DbType = "Int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
		public int Id { get; set; }

		[Column(DbType = "NVARCHAR(50) NOT NULL")]
		public string Name { get; set; }

		[Column(DbType = "NVARCHAR(50) NOT NULL")]
		public string Symbol { get; set; }

		[Column(Name = "AlgoTypeId", DbType = "tinyint NOT NULL")]
		public AlgoType AlgoType { get; set; }

		[Column(DbType = "tinyint NOT NULL")]
		public NetworkType NetworkType { get; set; }

		[Column(DbType = "decimal(38,8) NOT NULL")]
		public decimal Balance { get; set; }

		[Column(DbType = "nvarchar(128) NOT NULL")]
		public string WalletUser { get; set; }

		[Column(DbType = "nvarchar(128) NOT NULL")]
		public string WalletPass { get; set; }

		[Column(DbType = "int NOT NULL")]
		public int WalletPort { get; set; }

		[Column(DbType = "nvarchar(128) NOT NULL")]
		public string WalletHost { get; set; }

		[Column(DbType = "decimal(38,8) NOT NULL")]
		public decimal TradeFee { get; set; }

		[Column(DbType = "decimal(38,8)  NOT NULL")]
		public decimal MinTradeAmount { get; set; }

		[Column(DbType = "decimal(38,8)  NOT NULL")]
		public decimal MaxTradeAmount { get; set; }

		[Column(DbType = "decimal(38,8)  NOT NULL")]
		public decimal MinBaseTrade { get; set; }

		[Column(DbType = "decimal(38,8) NOT NULL")]
		public decimal TxFee { get; set; }

		[Column(DbType = "decimal(38,8) NOT NULL")]
		public decimal PoolFee { get; set; }

		[Column(DbType = "decimal(38,8)  NOT NULL")]
		public decimal WithdrawFee { get; set; }

		[Column(DbType = "TINYINT NOT NULL")]
		public WithdrawFeeType WithdrawFeeType { get; set; }

		[Column(DbType = "decimal(38,8)  NOT NULL")]
		public decimal MinWithdraw { get; set; }

		[Column(DbType = "decimal(38,8)  NOT NULL")]
		public decimal MaxWithdraw { get; set; }

		[Column(DbType = "decimal(38,8)  NOT NULL")]
		public decimal MinTip { get; set; }

		//[Column(DbType = "decimal(38,8)  NOT NULL")]
		//public decimal MinFaucet { get; set; }

		//[Column(DbType = "decimal(38,8)  NOT NULL")]
		//public decimal FaucetPercent { get; set; }

		[Column(DbType = "int NOT NULL")]
		public int MinConfirmations { get; set; }

		[Column(DbType = "Int NOT NULL")]
		public int Rank { get; set; }

		[Column(DbType = "TINYINT NOT NULL")]
		public CurrencyStatus Status { get; set; }

		[Column(DbType = "NVARCHAR(1024)")]
		public string StatusMessage { get; set; }

		[Column(DbType = "NVARCHAR(128) NULL")]
		public string LastBlockHash { get; set; }

		[Column(DbType = "NVARCHAR(128) NULL")]
		public string LastWithdrawBlockHash { get; set; }

		[Column(DbType = "Int NOT NULL")]
		public int ForumId { get; set; }

		[Column(DbType = "Int NOT NULL")]
		public int BlockTime { get; set; }

		[Column(DbType = "Int NOT NULL")]
		public int Block { get; set; }

		[Column(DbType = "NVARCHAR(4000)")]
		public string Summary { get; set; }

		[Column(DbType = "NVARCHAR(128) NULL")]
		public string Version { get; set; }

		[Column(DbType = "INT NOT NULL")]
		public int Connections { get; set; }

		[Column(DbType = "NVARCHAR(4000) NULL")]
		public string Errors { get; set; }

		[Column(DbType = "bit NOT NULL")]
		public bool IsEnabled { get; set; }

		[Column(DbType = "NVARCHAR(128) NULL")]
		public string BaseAddress { get; set; }

		[Column(DbType = "TINYINT NOT NULL")]
		public CurrencyType Type { get; set; }

		[Column(DbType = "TINYINT NOT NULL")]
		public InterfaceType InterfaceType { get; set; }

		[Column(DbType = "DateTime2 NOT NULL")]
		public DateTime FeaturedExpires { get; set; }

		[Column(DbType = "DateTime2 NOT NULL")]
		public DateTime TippingExpires { get; set; }

		[Column(DbType = "DateTime2 NOT NULL")]
		public DateTime RewardsExpires { get; set; }
	}
}
