using System;
using System.Data.Linq.Mapping;
using Cryptopia.Common.Webservice;

namespace Cryptopia.API.Objects
{
	[Table(Name = "dbo.MarketFeedback")]
    public class MarketFeedback : IIdentity<int>
    {
        [Column(AutoSync = AutoSync.OnInsert, DbType = "INT NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int Id { get; set; }

        [Column(DbType = "INT NOT NULL")]
        public int MarketItemId { get; set; }

        [Column(DbType = "UNIQUEIDENTIFIER NOT NULL")]
        public Guid SenderUserId { get; set; }

        [Column(DbType = "UNIQUEIDENTIFIER NOT NULL")]
        public Guid ReceiverUserId { get; set; }

        [Column(DbType = "INT NOT NULL")]
        public int Rating { get; set; }

        [Column(DbType = "NVARCHAR(256) NOT NULL")]
        public string Comment { get; set; }

        [Column(DbType = "DATETIME2 NOT NULL")]
        public DateTime Timestamp { get; set; }
    }
}
