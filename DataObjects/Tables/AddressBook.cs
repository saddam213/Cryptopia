using System;
using System.Data.Linq.Mapping;

namespace Cryptopia.API.Objects
{
	[Table(Name = "dbo.AddressBook")]
	public class AddressBook
	{
		[Column(AutoSync = AutoSync.OnInsert, DbType = "Int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
		public int Id { get; set; }

		[Column(DbType = "UNIQUEIDENTIFIER NOT NULL")]
		public Guid UserId { get; set; }

		[Column(DbType = "int NOT NULL")]
		public int CurrencyId { get; set; }

		[Column(DbType = "NVARCHAR(128) NOT NULL")]
		public string Label { get; set; }

		[Column(DbType = "NVARCHAR(128) NOT NULL")]
		public string Address { get; set; }

		[Column(DbType = "bit NOT NULL")]
		public bool IsEnabled { get; set; }
	}
}
