using System;
using System.Data.Linq.Mapping;
using System.Runtime.Serialization;
using Cryptopia.Common.Webservice;

namespace Cryptopia.API.Objects
{
    [DataContract]
    [Table(Name = "dbo.Balance")]
    public class Balance : IIdentity<int>
    {
        [DataMember]
        [Column(AutoSync = AutoSync.OnInsert, DbType = "Int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int Id { get; set; }

        [DataMember]
        [Column(DbType = "UNIQUEIDENTIFIER NOT NULL")]
        public Guid UserId { get; set; }

        [DataMember]
        [Column(DbType = "Int NOT NULL")]
        public int CurrencyId { get; set; }

        [DataMember]
        [Column(DbType = "decimal(38,8) NOT NULL")]
        public decimal Total { get; set; }

        [DataMember]
        [Column(DbType = "decimal(38,8) NOT NULL")]
        public decimal Unconfirmed { get; set; }

        [DataMember]
        [Column(DbType = "decimal(38,8) NOT NULL")]
        public decimal HeldForTrades { get; set; }

        [DataMember]
        [Column(DbType = "decimal(38,8) NOT NULL")]
        public decimal PendingWithdraw { get; set; }

        [IgnoreDataMember]
        public decimal Avaliable
        {
            get { return Total - (Unconfirmed + HeldForTrades + PendingWithdraw); }
        }
    }
}
