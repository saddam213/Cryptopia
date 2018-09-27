using System.Data.Linq.Mapping;
using System.Runtime.Serialization;
using Cryptopia.Common.Webservice;
using Cryptopia.Enums;

namespace Cryptopia.API.Objects
{
	[DataContract]
	[Table(Name = "dbo.TradePair")]
	public class TradePair : IIdentity<int>
	{
		[DataMember]
		[Column(AutoSync = AutoSync.OnInsert, DbType = "Int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
		public int Id { get; set; }

		[DataMember]
		[Column(DbType = "Int NOT NULL")]
		public int CurrencyId1 { get; set; }

		[DataMember]
		[Column(DbType = "Int NOT NULL")]
		public int CurrencyId2 { get; set; }

		[DataMember]
		[Column(DbType = "decimal(38,8) NOT NULL")]
		public decimal LastTrade { get; set; }

		[DataMember]
		[Column(DbType = "float NOT NULL")]
		public double Change { get; set; }

		[DataMember]
		[Column(Name = "StatusId", DbType = "TINYINT NOT NULL")]
		public TradePairStatus Status { get; set; }

		[DataMember]
		[Column(DbType = "NVARCHAR(1024)")]
		public string StatusMessage { get; set; }
	}
}