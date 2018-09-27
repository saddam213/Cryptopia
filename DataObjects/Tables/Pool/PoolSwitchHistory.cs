using System;
using System.Data.Linq.Mapping;
using Cryptopia.Enums;

namespace Cryptopia.API.Objects
{
	[Table(Name = "dbo.PoolSwitchHistory")]
	public class PoolSwitchHistory
	{
		[Column(AutoSync = AutoSync.OnInsert, DbType = "Int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
		public int Id { get; set; }

		[Column(DbType = "tinyint NOT NULL")]
		public AlgoType AlgoType { get; set; }

		[Column(DbType = "nvarchar(128) NOT NULL")]
		public string TablePrefix { get; set; }

		[Column(DbType = "decimal(38,8) NOT NULL")]
		public decimal Profitability { get; set; }

		[Column(DbType = "DATETIME2 NOT NULL")]
		public DateTime Timestamp { get; set; }
	}
}