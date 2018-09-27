using System;
using System.Data.Linq.Mapping;
using System.Runtime.Serialization;
using Cryptopia.Common.Webservice;

namespace Cryptopia.API.Objects
{
	[DataContract]
	[Table(Name = "dbo.Address")]
	public class Address : IIdentity<int>
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
		[Column(Name = "Address", DbType = "NVARCHAR(128) NOT NULL", CanBeNull = false)]
		public string AddressHash { get; set; }

		[DataMember]
		[Column(DbType = "NVARCHAR(256) NOT NULL")]
		public string PrivateKey { get; set; }
	}
}