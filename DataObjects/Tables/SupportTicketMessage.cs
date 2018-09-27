using System;
using System.Data.Linq.Mapping;
using Cryptopia.Common.Webservice;

namespace Cryptopia.API.Objects
{
	[Table(Name = "dbo.SupportTicketMessage")]
    public class SupportTicketMessage : IIdentity<int>
    {
        [Column(AutoSync = AutoSync.OnInsert, DbType = "Int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int Id { get; set; }

        [Column(DbType = "Int NOT NULL")]
        public int TicketId { get; set; }

        [Column(DbType = "NVARCHAR(256) NOT NULL")]
        public string Sender { get; set; }

        [Column(DbType = "NVARCHAR(MAX) NOT NULL")]
        public string Message { get; set; }

        [Column(DbType = "bit NOT NULL")]
        public bool IsAdminReply { get; set; }

        [Column(DbType = "datetime2 NOT NULL")]
        public DateTime TimeStamp { get; set; }
    }
}
