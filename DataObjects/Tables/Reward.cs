using System;
using System.Data.Linq.Mapping;
using Cryptopia.Common.Webservice;

namespace Cryptopia.API.Objects
{
	[Table(Name = "dbo.Reward")]
	public class Reward : IIdentity<int>
	{
		[Column(AutoSync = AutoSync.OnInsert, DbType = "Int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
		public int Id { get; set; }

		[Column(DbType = "UNIQUEIDENTIFIER NOT NULL")]
		public Guid UserId { get; set; }

		[Column(DbType = "int NOT NULL")]
		public int CurrencyId { get; set; }

		[Column(DbType = "DECIMAL(38,8) NOT NULL")]
		public decimal Amount { get; set; }

		[Column(DbType = "DECIMAL(38,8) NOT NULL")]
		public decimal Percent { get; set; }

		[Column(DbType = "NVARCHAR(128) NOT NULL")]
		public string RewardType { get; set; }

		[Column(DbType = "NVARCHAR(256) NOT NULL")]
		public string Message { get; set; }

		[Column(DbType = "datetime2 NOT NULL")]
		public DateTime TimeStamp { get; set; }
	}
}