using System.Data.Linq.Mapping;
using Cryptopia.Enums;

namespace Cryptopia.API.Objects
{
	[Table(Name = "dbo.PoolSwitch")]
	public class PoolSwitch
	{
		[Column(AutoSync = AutoSync.OnInsert, DbType = "Int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
		public int Id { get; set; }

		[Column(DbType = "tinyint NOT NULL")]
		public AlgoType AlgoType { get; set; }

		[Column(DbType = "nvarchar(50) NULL")]
		public string TablePrefix { get; set; }

		[Column(DbType = "decimal(38,8) NOT NULL")]
		public decimal Profitability { get; set; }

		[Column(DbType = "tinyint NOT NULL")]
		public CurrencyStatus Status { get; set; }

		[Column(DbType = "nvarchar(1024) NULL")]
		public string StatusMessage { get; set; }

		[Column(DbType = "bit NOT NULL")]
		public bool IsEnabled { get; set; }
	}

	public enum PoolTargetBits : byte
	{
		Scrypt = 16,
		X11 = 24,
		sha256 = 32,
	}
}