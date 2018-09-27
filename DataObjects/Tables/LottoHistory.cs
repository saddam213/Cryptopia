using Cryptopia.Common.Webservice;
using System;
using System.Data.Linq.Mapping;

namespace Cryptopia.API.Objects
{
	[Table(Name = "dbo.LottoHistory")]
	public class LottoHistory : IIdentity<int>
	{
		[Column(AutoSync = AutoSync.OnInsert, DbType = "Int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
		public int Id { get; set; }

		[Column(DbType = "UNIQUEIDENTIFIER NOT NULL")]
		public Guid UserId { get; set; }

		[Column(DbType = "INT NOT NULL")]
		public int LottoItemId { get; set; }

		[Column(DbType = "INT NOT NULL")]
		public int LottoTicketId { get; set; }

		[Column(DbType = "INT NOT NULL")]
		public int LottoDrawId { get; set; }

		[Column(DbType = "INT NOT NULL")]
		public int Position { get; set; }

		[Column(DbType = "DECIMAL(38,8) NOT NULL")]
		public decimal Amount { get; set; }

		[Column(DbType = "DECIMAL(38,8) NOT NULL")]
		public decimal Percent { get; set; }

		[Column(DbType = "DECIMAL(38,8) NOT NULL")]
		public decimal TotalAmount { get; set; }
		[Column(DbType = "DATETIME2 NOT NULL")]
		public DateTime Timestamp { get; set; }
	}
}
