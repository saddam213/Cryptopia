using System;
using System.Data.Linq.Mapping;
using System.Runtime.Serialization;
using Cryptopia.Common.Webservice;
using Cryptopia.Enums;

namespace Cryptopia.API.Objects
{
	[DataContract]
	[Table(Name = "dbo.Deposit")]
	public class Deposit : IIdentity<int>
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
		[Column(DbType = "decimal(38,8) NOT NULL")]
		public decimal Amount { get; set; }

		[DataMember]
		[Column(DbType = "nvarchar(256) NOT NULL")]
		public string Txid { get; set; }

		[DataMember]
		[Column(DbType = "int NOT NULL")]
		public int Confirmations { get; set; }

		[DataMember]
		[Column(Name = "DepositTypeId", DbType = "tinyint NOT NULL")]
		public DepositType DepositType { get; set; }

		[DataMember]
		[Column(Name = "DepositStatusId", DbType = "tinyint NOT NULL")]
		public DepositStatus DepositStatus { get; set; }

		[DataMember]
		[Column(DbType = "datetime2 NOT NULL")]
		public DateTime TimeStamp { get; set; }
	}
}