using System;
using System.Data.Linq.Mapping;

namespace Cryptopia.API.Objects
{
	[Table(Name = "dbo.PoolWorker")]
    public class PoolWorker
    {  
        [Column(AutoSync = AutoSync.OnInsert, DbType = "Int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int Id { get; set; }
      
        [Column(DbType = "uniqueidentifier NOT NULL")]
        public Guid UserId { get; set; }

        [Column(DbType = "nvarchar(50) NOT NULL")]
        public string Name { get; set; }

        [Column(DbType = "nvarchar(50) NOT NULL")]
        public string Password { get; set; }

        [Column(DbType = "float NOT NULL")]
        public double Difficulty { get; set; }

        [Column(DbType = "float NOT NULL")]
        public double Hashrate { get; set; }

        [Column(DbType = "int NOT NULL")]
        public int ActivePort { get; set; }

        [Column(DbType = "DATETIME2 NOT NULL")]
        public DateTime LastShareTime { get; set; }

        [Column(DbType = "BIT NOT NULL")]
        public bool IsEnabled { get; set; }
    }
}