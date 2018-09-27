using System;
using System.Data.Linq.Mapping;

namespace Cryptopia.API.Objects
{
	[Table(Name = "dbo.Settings")]
	public class Settings
	{
		[Column(AutoSync = AutoSync.OnInsert, DbType = "INT NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
		public int Id { get; set; }

		[Column(DbType = "DECIMAL(38,8) NOT NULL")]
		public decimal PayBanPrice { get; set; }
	}
}
