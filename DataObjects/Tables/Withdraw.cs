using System;
using System.Data.Linq.Mapping;
using System.Runtime.Serialization;
using Cryptopia.Common.Webservice;
using Cryptopia.Enums;

namespace Cryptopia.API.Objects
{
	[DataContract]
	[Table(Name = "dbo.Withdraw")]
	public class Withdraw : IIdentity<int>
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
		[Column(DbType = "NVARCHAR(256) NOT NULL")]
		public string Address { get; set; }

		[DataMember]
		[Column(DbType = "decimal(38,8) NOT NULL")]
		public decimal Amount { get; set; }

		[DataMember]
		[Column(DbType = "decimal(38,8) NOT NULL")]
		public decimal Fee { get; set; }

		[DataMember]
		[Column(DbType = "int NOT NULL")]
		public int Confirmations { get; set; }

		[DataMember]
		[Column(DbType = "nvarchar(128) NOT NULL")]
		public string Txid { get; set; }

		[DataMember]
		[Column(Name = "WithdrawTypeId", DbType = "tinyint NOT NULL")]
		public WithdrawType WithdrawType { get; set; }

		[DataMember]
		[Column(Name = "WithdrawStatusId", DbType = "tinyint NOT NULL")]
		public WithdrawStatus WithdrawStatus { get; set; }

		[DataMember]
		[Column(DbType = "nvarchar(256) NOT NULL")]
		public string TwoFactorToken { get; set; }

		[DataMember]
		[Column(DbType = "BIT NOT NULL")]
		public bool IsApi { get; set; }

		[DataMember]
		[Column(DbType = "datetime2 NOT NULL")]
		public DateTime TimeStamp { get; set; }
	}
}