using System;
using System.Data.Linq.Mapping;
using System.Runtime.Serialization;
using Cryptopia.Common.Webservice;

namespace Cryptopia.API.Objects
{
    [DataContract]
	[Table(Name = "dbo.FaucetClaim")]
    public class FaucetClaim : IIdentity<int>
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
        [Column(DbType = "datetime2 NOT NULL")]
        public DateTime TimeStamp { get; set; }
    }
}