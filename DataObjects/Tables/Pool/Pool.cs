using Cryptopia.API.DataObjects;
using System.Data.Linq.Mapping;

namespace Cryptopia.API.Objects
{
	[Table(Name = "dbo.Pool")]
    public class Pool
    {
        [Column(AutoSync = AutoSync.OnInsert, DbType = "Int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int Id { get; set; }

        [Column(DbType = "int NOT NULL")]
        public int CurrencyId { get; set; }

		[Column(DbType = "nvarchar(10) NOT NULL")]
		public string Symbol { get; set; }

		[Column(DbType = "nvarchar(10) NOT NULL")]
		public string Name { get; set; }

        [Column(DbType = "tinyint NOT NULL")]
		public PoolAlgoType AlgoType { get; set; }

        [Column(DbType = "nvarchar(10) NOT NULL")]
        public string TablePrefix { get; set; }
    
        [Column(DbType = "tinyint NOT NULL")]
        public PoolTargetBits TargetBits { get; set; }

		[Column(DbType = "decimal(38,8)  NOT NULL")]
		public decimal BlockReward { get; set; }

        [Column(DbType = "nvarchar(128) NOT NULL")]
        public string WalletUser { get; set; }

        [Column(DbType = "nvarchar(128) NOT NULL")]
        public string WalletPass { get; set; }

        [Column(DbType = "int NOT NULL")]
        public int WalletPort { get; set; }

        [Column(DbType = "nvarchar(128) NOT NULL")]
        public string WalletHost { get; set; }

        [Column(DbType = "int NOT NULL")]
        public int Fee { get; set; }

        [Column(DbType = "decimal(38,8)  NOT NULL")]
        public decimal MinPayout { get; set; }

        [Column(DbType = "decimal(38,8) NOT NULL")]
        public decimal MinLiquidPayout { get; set; }

        [Column(DbType = "nvarchar(128) NOT NULL")]
        public string LiquidAddress { get; set; }

        [Column(DbType = "int NOT NULL")]
        public int PayoutPeriod { get; set; }

        [Column(DbType = "int NOT NULL")]
        public int ProcessingPollPeriod { get; set; }

        [Column(DbType = "int NOT NULL")]
        public int StatsCalculationPeriod { get; set; }

        [Column(DbType = "nvarchar(1024) NULL")]
        public string SpecialInstructions { get; set; }

        [Column(DbType = "tinyint NOT NULL")]
		public PoolStatus Status { get; set; }

        [Column(DbType = "nvarchar(1024) NULL")]
        public string StatusMessage { get; set; }

		[Column(DbType = "bit NOT NULL")]
		public bool IsFeatured { get; set; }

        [Column(DbType = "bit NOT NULL")]
        public bool IsEnabled { get; set; }
    }


	public enum PoolStatus : byte
	{
		OK = 0,
		Maintenance = 1,
		Offline = 2
	}
}