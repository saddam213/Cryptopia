using System;
using System.Data.Linq.Mapping;
using System.Runtime.Serialization;
using Cryptopia.Common.Webservice;
using Cryptopia.Enums;

namespace Cryptopia.API.Objects
{
    [DataContract]
    [Table(Name = "dbo.MarketItem")]
    public class MarketItem : IIdentity<int>
    {
        [DataMember]
        [Column(AutoSync = AutoSync.OnInsert, DbType = "INT NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int Id { get; set; }

        [DataMember]
        [Column(DbType = "UNIQUEIDENTIFIER NOT NULL")]
        public Guid UserId { get; set; }

        [DataMember]
        [Column(DbType = "INT NOT NULL")]
        public int CategoryId { get; set; }

        [DataMember]
        [Column(DbType = "INT NOT NULL")]
        public int CurrencyId { get; set; }

        [DataMember]
        [Column(Name = "MarketItemTypeId", DbType = "TINYINT NOT NULL")]
        public MarketItemType Type { get; set; }

        [DataMember]
        [Column(Name = "MarketItemStatusId", DbType = "TINYINT NOT NULL")]
        public MarketItemStatus Status { get; set; }

        [DataMember]
        [Column(Name = "MarketItemFeatureId", DbType = "TINYINT NOT NULL")]
        public MarketItemFeature Feature { get; set; }

        [DataMember]
        [Column(DbType = "NVARCHAR(128) NOT NULL")]
        public string Title { get; set; }

        [DataMember]
        [Column(DbType = "NVARCHAR(MAX) NOT NULL")]
        public string Description { get; set; }

        [DataMember]
        [Column(DbType = "NVARCHAR(256) NOT NULL")]
        public string MainImage { get; set; }

        [DataMember]
        [Column(DbType = "DECIMAL(38,8) NOT NULL")]
        public decimal AskingPrice { get; set; }

        [DataMember]
        [Column(DbType = "DECIMAL(38,8) NOT NULL")]
        public decimal ReservePrice { get; set; }

        [DataMember]
        [Column(DbType = "INT NOT NULL")]
        public int LocationId { get; set; }

        [DataMember]
        [Column(DbType = "NVARCHAR(128) NOT NULL")]
        public string LocationRegion { get; set; }

        [Column(DbType = "BIT NOT NULL")]
        public bool AllowPickup { get; set; }

        [Column(DbType = "BIT NOT NULL")]
        public bool PickupOnly { get; set; }

        [Column(DbType = "BIT NOT NULL")]
        public bool ShippingBuyerArrange { get; set; }

        [Column(DbType = "BIT NOT NULL")]
        public bool ShippingNational { get; set; }

        [Column(DbType = "BIT NOT NULL")]
        public bool ShippingInternational { get; set; }

        [Column(DbType = "DECIMAL(38,8) NOT NULL")]
        public decimal ShippingNationalPrice { get; set; }

        [Column(DbType = "NVARCHAR(128) NOT NULL")]
        public string ShippingNationalDetails { get; set; }

        [Column(DbType = "DECIMAL(38,8) NOT NULL")]
        public decimal ShippingInternationalPrice { get; set; }

        [Column(DbType = "NVARCHAR(128) NOT NULL")]
        public string ShippingInternationalDetails { get; set; }

        [DataMember]
        [Column(DbType = "DATETIME2 NOT NULL")]
        public DateTime CloseDate { get; set; }

        [DataMember]
        [Column(DbType = "DATETIME2 NOT NULL")]
        public DateTime Created { get; set; }
    }
}
