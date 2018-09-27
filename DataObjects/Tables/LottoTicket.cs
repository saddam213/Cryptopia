using System;
using System.Data.Linq.Mapping;
using Cryptopia.Common.Webservice;

namespace Cryptopia.API.Objects
{
	[Table(Name = "dbo.LottoTicket")]
	public class LottoTicket : IIdentity<int>
	{
		[Column(AutoSync = AutoSync.OnInsert, DbType = "Int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
		public int Id { get; set; }

		[Column(DbType = "UNIQUEIDENTIFIER NOT NULL")]
		public Guid UserId { get; set; }

		[Column(DbType = "Int NOT NULL")]
		public int DrawId { get; set; }

		[Column(DbType = "Int NOT NULL")]
		public int LottoItemId { get; set; }

		[Column(DbType = "DateTime2 NOT NULL")]
		public DateTime Timestamp { get; set; }

		[Column(DbType = "BIT NOT NULL")]
		public bool IsArchived { get; set; }
	}
}
