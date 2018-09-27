using System;
using System.Data.Linq.Mapping;
using Cryptopia.Common.Webservice;

namespace Cryptopia.API.Objects
{
	[Table(Name = "dbo.PaymentItem")]
	public class PaymentItem : IIdentity<int>
	{
		[Column(AutoSync = AutoSync.OnInsert, DbType = "INT NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
		public int Id { get; set; }

		[Column(DbType = "UNIQUEIDENTIFIER NOT NULL")]
		public Guid UserId { get; set; }

		[Column(DbType = "INT NOT NULL")]
		public int CurrencyId { get; set; }

		[Column(DbType = "TINYINT NOT NULL")]
		public PaymentItemType Type { get; set; }

		[Column(DbType = "TINYINT NOT NULL")]
		public PaymentItemStatus Status { get; set; }

		[Column(DbType = "NVARCHAR(128) NOT NULL")]
		public string Title { get; set; }

		[Column(DbType = "NVARCHAR(MAX) NOT NULL")]
		public string Description { get; set; }

		[Column(DbType = "DATETIME2 NOT NULL")]
		public DateTime Created { get; set; }
	}
}
