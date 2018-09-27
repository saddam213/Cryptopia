using System;
using System.Data.Linq.Mapping;

namespace Cryptopia.API.Objects
{
	[Table(Name = "dbo.PoolInfo")]
    public class PoolInfo
    {  
        [Column(AutoSync = AutoSync.OnInsert, DbType = "Int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int Id { get; set; }
      
        [Column(DbType = "int NOT NULL")]
        public int PoolId { get; set; }

        [Column(DbType = "float NOT NULL")]
        public double Hashrate { get; set; }

        [Column(DbType = "float NOT NULL")]
        public double NetworkHashrate { get; set; }

        [Column(DbType = "float NOT NULL")]
        public double NetworkDifficulty { get; set; }

        [Column(DbType = "float NOT NULL")]
        public double BlockProgress { get; set; }

        [Column(DbType = "int NOT NULL")]
        public int CurrentBlock { get; set; }

        [Column(DbType = "int NOT NULL")]
        public int LastPoolBlock { get; set; }

        [Column(DbType = "DateTime2 NOT NULL")]
        public DateTime LastBlockTime { get; set; }

        [Column(DbType = "int NOT NULL")]
        public int EstimatedTime { get; set; }

        [Column(DbType = "float NOT NULL")]
        public double EstimatedShares { get; set; }

        [Column(DbType = "float NOT NULL")]
        public double ValidShares { get; set; }

        [Column(DbType = "float NOT NULL")]
        public double InvalidShares { get; set; }

		[Column(DbType = "decimal(38,8) NOT NULL")]
		public decimal Profitability { get; set; }

        [Column(DbType = "DateTime2 NOT NULL")]
        public DateTime NextPayout { get; set; }

        [Column(DbType = "NVARCHAR(128) NULL")]
        public string LastBlockHash { get; set; }
    }
}