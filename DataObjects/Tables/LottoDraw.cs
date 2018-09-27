using Cryptopia.Common.Webservice;
using System;
using System.Data.Linq.Mapping;

namespace Cryptopia.API.Objects
{
	[Table(Name = "dbo.LottoDraw")]
	public class LottoDraw : IIdentity<int>
	{
		[Column(AutoSync = AutoSync.OnInsert, DbType = "Int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
		public int Id { get; set; }

		[Column(DbType = "INT NOT NULL")]
		public int LottoDrawId { get; set; }

		[Column(DbType = "INT NOT NULL")]
		public int LottoItemId { get; set; }

		[Column(DbType = "INT NOT NULL")]
		public int TicketCount { get; set; }
		
		[Column(DbType = "DATETIME2 NOT NULL")]
		public DateTime Timestamp { get; set; }
	}
}
