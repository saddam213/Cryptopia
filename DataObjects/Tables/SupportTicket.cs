using System;
using System.Data.Linq.Mapping;
using Cryptopia.Common.Webservice;
using Cryptopia.Enums;

namespace Cryptopia.API.Objects
{
	[Table(Name = "dbo.SupportTicket")]
	public class SupportTicket : IIdentity<int>
	{
		[Column(AutoSync = AutoSync.OnInsert, DbType = "Int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
		public int Id { get; set; }

		[Column(DbType = "UNIQUEIDENTIFIER NOT NULL")]
		public Guid UserId { get; set; }

		[Column(DbType = "NVARCHAR(256) NOT NULL")]
		public string Title { get; set; }

		[Column(DbType = "NVARCHAR(MAX) NOT NULL")]
		public string Description { get; set; }

		[Column(Name = "StatusId", DbType = "tinyint NOT NULL")]
		public SupportTicketStatus Status { get; set; }

		[Column(Name = "CategoryId", DbType = "tinyint NOT NULL")]
		public SupportTicketCategory Category { get; set; }

		[Column(DbType = "datetime2")]
		public DateTime LastUpdate { get; set; }

		[Column(DbType = "datetime2")]
		public DateTime Created { get; set; }
	}
}