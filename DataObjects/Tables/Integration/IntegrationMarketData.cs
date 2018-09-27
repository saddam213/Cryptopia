using Cryptopia.Common.Webservice;
using System;
using System.Data.Linq.Mapping;

namespace Cryptopia.API.Objects
{
	[Table(Name = "dbo.IntegrationMarketData")]
	public class IntegrationMarketData : IIdentity<int>
	{
		[Column(AutoSync = AutoSync.OnInsert, DbType = "Int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
		public int Id { get; set; }

		[Column(DbType = "Int NOT NULL")]
		public int IntegrationExchangeId { get; set; }

		[Column(DbType = "Int NOT NULL")]
		public int TradePairId { get; set; }

		[Column(DbType = "decimal(38,8) NOT NULL")]
		public decimal Bid { get; set; }

		[Column(DbType = "decimal(38,8) NOT NULL")]
		public decimal Ask { get; set; }

		[Column(DbType = "decimal(38,8) NOT NULL")]
		public decimal Last { get; set; }

		[Column(DbType = "decimal(38,8) NOT NULL")]
		public decimal Volume { get; set; }

		[Column(DbType = "decimal(38,8) NOT NULL")]
		public decimal BaseVolume { get; set; }

		[Column(DbType = "NVARCHAR(256) NULL")]
		public string MarketUrl { get; set; }

		[Column(DbType = "datetime2 NOT NULL")]
		public DateTime Timestamp { get; set; }
	}
}