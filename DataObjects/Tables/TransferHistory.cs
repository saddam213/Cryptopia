using System;
using System.Data.Linq.Mapping;
using System.Runtime.Serialization;
using Cryptopia.Common.Webservice;
using Cryptopia.Enums;

namespace Cryptopia.API.Objects
{
	[DataContract]
	[Table(Name = "dbo.TransferHistory")]
	public class TransferHistory : IIdentity<int>
	{
		[DataMember]
		[Column(AutoSync = AutoSync.OnInsert, DbType = "Int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
		public int Id { get; set; }

		[DataMember]
		[Column(DbType = "UNIQUEIDENTIFIER NOT NULL")]
		public Guid UserId { get; set; }

		[DataMember]
		[Column(DbType = "UNIQUEIDENTIFIER NOT NULL")]
		public Guid ToUserId { get; set; }

		[DataMember]
		[Column(DbType = "Int NOT NULL")]
		public int CurrencyId { get; set; }

		[DataMember]
		[Column(DbType = "tinyint NOT NULL")]
		public TransferType TransferType { get; set; }

		[DataMember]
		[Column(DbType = "decimal(38,8) NOT NULL")]
		public decimal Amount { get; set; }

		[DataMember]
		[Column(DbType = "decimal(38,8) NOT NULL")]
		public decimal Fee { get; set; }

		[DataMember]
		[Column(DbType = "DateTime2 NOT NULL")]
		public DateTime Timestamp { get; set; }
	}
}