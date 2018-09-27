using System;
using System.Data.Linq.Mapping;
using Cryptopia.Common.Webservice;
using Cryptopia.Enums;

namespace Cryptopia.API.Objects
{
	[Table(Name = "dbo.LottoItem")]
	public class LottoItem : IIdentity<int>
	{
		[Column(AutoSync = AutoSync.OnInsert, DbType = "Int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
		public int Id { get; set; }

		[Column(DbType = "NVARCHAR(128) NOT NULL")]
		public string Name { get; set; }

		[Column(DbType = "NVARCHAR(4000) NULL")]
		public string Description { get; set; }

		[Column(DbType = "INT NOT NULL")]
		public int CurrencyId { get; set; }

		[Column(DbType = "INT NOT NULL")]
		public int Prizes { get; set; }

		[Column(DbType = "DECIMAL(38,8) NOT NULL")]
		public decimal Rate { get; set; }

		[Column(DbType = "DECIMAL(38,8) NOT NULL")]
		public decimal Fee { get; set; }

		[Column(DbType = "INT NOT NULL")]
		public int Hours { get; set; }

		[Column(DbType = "TINYINT NOT NULL")]
		public LottoType LottoType { get; set; }
				
		[Column(DbType = "DATETIME2 NOT NULL")]
		public DateTime NextDraw { get; set; }

		[Column(DbType = "INT NOT NULL")]
		public int CurrentDrawId { get; set; }

		[Column(DbType = "TINYINT NOT NULL")]
		public LottoItemStatus Status { get; set; }

		[Column(DbType = "DECIMAL(38,8) NOT NULL")]
		public decimal PrizePool { get; set; }
	}
}