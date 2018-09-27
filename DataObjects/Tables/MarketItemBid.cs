using System;
using System.Data.Linq.Mapping;
using Cryptopia.Common.Webservice;

namespace Cryptopia.API.Objects
{
	[Table(Name = "dbo.MarketItemBid")]
    public class MarketItemBid : IIdentity<int>
    {
        [Column(AutoSync = AutoSync.OnInsert, DbType = "INT NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int Id { get; set; }

        [Column(DbType = "UNIQUEIDENTIFIER NOT NULL")]
        public Guid UserId { get; set; }

        [Column(DbType = "INT NOT NULL")]
        public int MarketItemId { get; set; }

        [Column(DbType = "DECIMAL(38,8) NOT NULL")]
        public decimal BidAmount { get; set; }

        [Column(DbType = "BIT NOT NULL")]
        public bool IsWinningBid { get; set; }

        [Column(DbType = "DATETIME2 NOT NULL")]
        public DateTime Timestamp { get; set; }
    }
}
