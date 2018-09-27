using System;
using System.Data.Linq.Mapping;

namespace Cryptopia.API.Objects
{
	[Table(Name = "dbo.PoolUserInfo")]
	public class PoolUserInfo
    {  
        [Column(AutoSync = AutoSync.OnInsert, DbType = "Int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int Id { get; set; }

        [Column(DbType = "uniqueidentifier NOT NULL")]
        public Guid UserId { get; set; }

        [Column(DbType = "int NOT NULL")]
        public int PoolId { get; set; }

		[Column(DbType = "float NOT NULL")]
		public double Hashrate { get; set; }

        [Column(DbType = "float NOT NULL")]
        public double ValidShares { get; set; }

        [Column(DbType = "float NOT NULL")]
        public double InvalidShares { get; set; }

        [Column(DbType = "decimal(38,8) NOT NULL")]
        public decimal Confirmed { get; set; }

        [Column(DbType = "decimal(38,8) NOT NULL")]
        public decimal Unconfirmed { get; set; }

		[Column(DbType = "BIT NOT NULL")]
		public bool IsActive { get; set; }

		[Column(DbType = "DATETIME2 NOT NULL")]
		public DateTime LastShareTime { get; set; }
    }
}