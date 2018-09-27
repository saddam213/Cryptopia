using System;
using System.Data.Linq.Mapping;
using System.Runtime.Serialization;
using Cryptopia.Common.Webservice;

namespace Cryptopia.API.Objects
{
    [Table(Name = "dbo.PaymentHistory")]
	public class PaymentHistory : IIdentity<int>
    {
        [DataMember]
        [Column(AutoSync = AutoSync.OnInsert, DbType = "INT NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int Id { get; set; }

		[DataMember]
		[Column(DbType = "INT NOT NULL")]
		public int PaymentItemId { get; set; }

        [DataMember]
        [Column(DbType = "UNIQUEIDENTIFIER NOT NULL")]
        public Guid UserId { get; set; }

        [DataMember]
        [Column(DbType = "DECIMAL(38,8) NOT NULL")]
        public decimal Amount { get; set; }

        [DataMember]
        [Column(DbType = "DATETIME2 NOT NULL")]
        public DateTime Created { get; set; }
    }
}
