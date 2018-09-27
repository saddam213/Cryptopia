using System;
using System.Data.Linq.Mapping;
using System.Runtime.Serialization;
using Cryptopia.Common.Webservice;

namespace Cryptopia.API.Objects
{
    [DataContract]
    [Table(Name = "dbo.DepositAutoSell")]
    public class DepositAutoSell : IIdentity<int>
    {
        [DataMember]
        [Column(AutoSync = AutoSync.OnInsert, DbType = "Int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int Id { get; set; }

        [DataMember]
        [Column(DbType = "UNIQUEIDENTIFIER NOT NULL")]
        public Guid UserId { get; set; }

        [DataMember]
        [Column(DbType = "int NOT NULL")]
        public int CurrencyId { get; set; }

        [DataMember]
        [Column(DbType = "tinyint NOT NULL")]
        public DepositAutoSellType AutoSellType { get; set; }

        [DataMember]
        [Column(DbType = "tinyint NOT NULL")]
        public DepositAutoSellActionType AutoSellActionType { get; set; }

        [DataMember]
        [Column(DbType = "int NOT NULL")]
        public int BuyCurrencyId { get; set; }

        [DataMember]
        [Column(DbType = "DECIMAL(38,8) NOT NULL")]
        public decimal MaxBuyPrice { get; set; }

        [DataMember]
        [Column(DbType = "DECIMAL(38,8) NOT NULL")]
        public decimal BuyOrderPrice { get; set; }

        [DataMember]
        [Column(DbType = "datetime2 NOT NULL")]
        public DateTime TimeStamp { get; set; }

        [DataMember]
        [Column(DbType = "BIT NOT NULL")]
        public bool IsEnabled { get; set; }
    }

    public enum DepositAutoSellType : short
    {
        Normal = 0,
        MultiPool = 1
    }

    public enum DepositAutoSellActionType : short
    {
        BuyIfBelowKeepIfAbove = 0,
        BuyIfBelowPlaceIfAbove = 1
    }
}