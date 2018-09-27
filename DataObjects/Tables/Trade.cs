using System;
using System.Data.Linq.Mapping;
using System.Runtime.Serialization;
using Cryptopia.Common.Webservice;
using Cryptopia.Enums;

namespace Cryptopia.API.Objects
{
	[DataContract]
	[Table(Name = "dbo.Trade")]
	public class Trade : IIdentity<int>
	{
		[DataMember]
		[Column(AutoSync = AutoSync.OnInsert, DbType = "Int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
		public int Id { get; set; }

		[DataMember]
		[Column(DbType = "UNIQUEIDENTIFIER NOT NULL")]
		public Guid UserId { get; set; }

		[DataMember]
		[Column(DbType = "Int NOT NULL")]
		public int TradePairId { get; set; }

		[DataMember]
		[Column(Name = "TradeTypeId", DbType = "tinyint NOT NULL")]
		public TradeHistoryType TradeType { get; set; }

		[DataMember]
		[Column(DbType = "decimal(38,8) NOT NULL")]
		public decimal Amount { get; set; }

		[DataMember]
		[Column(DbType = "decimal(38,8) NOT NULL")]
		public decimal Rate { get; set; }

		[DataMember]
		[Column(DbType = "decimal(38,8) NOT NULL")]
		public decimal Fee { get; set; }

		[DataMember]
		[Column(DbType = "DateTime2 NOT NULL")]
		public DateTime Timestamp { get; set; }

		[DataMember]
		[Column(Name = "TradeStatusId", DbType = "tinyint NOT NULL")]
		public TradeStatus Status { get; set; }

		[DataMember]
		[Column(DbType = "decimal(38,8) NOT NULL")]
		public decimal Remaining { get; set; }

		[Column(DbType = "BIT NOT NULL")]
		public bool IsApi { get; set; }
	}
}