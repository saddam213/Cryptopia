using System;
using System.Data.Linq.Mapping;
using System.Runtime.Serialization;
using Cryptopia.Common.Webservice;

namespace Cryptopia.API.Objects
{
    [DataContract]
    [Table(Name = "dbo.MarketItemQuestion")]
    public class MarketItemQuestion : IIdentity<int>
    {
        [DataMember]
        [Column(AutoSync = AutoSync.OnInsert, DbType = "INT NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int Id { get; set; }

        [DataMember]
        [Column(DbType = "UNIQUEIDENTIFIER NOT NULL")]
        public Guid UserId { get; set; }
     
        [DataMember]
        [Column(DbType = "INT NOT NULL")]
        public int MarketItemId { get; set; }

        [DataMember]
        [Column(DbType = "NVARCHAR(1024) NOT NULL")]
        public string Question { get; set; }

        [DataMember]
        [Column(DbType = "NVARCHAR(1024) NOT NULL")]
        public string Answer { get; set; }

        [DataMember]
        [Column(DbType = "DATETIME2 NOT NULL")]
        public DateTime Timestamp { get; set; }
    }
}
