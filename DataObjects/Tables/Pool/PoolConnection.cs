using System.Data.Linq.Mapping;

namespace Cryptopia.API.Objects
{
	[Table(Name = "dbo.PoolConnection")]
    public class PoolConnection
    {
        [Column(AutoSync = AutoSync.OnInsert, DbType = "Int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int Id { get; set; }

        [Column(DbType = "int NOT NULL")]
        public int PoolId { get; set; }

        [Column(DbType = "nvarchar(128) NOT NULL")]
        public string Name { get; set; }

        [Column(DbType = "nvarchar(128) NOT NULL")]
        public string StratumUrl { get; set; }

        [Column(DbType = "int NOT NULL")]
        public int Port { get; set; }

        [Column(DbType = "bit NOT NULL")]
        public bool IsEnabled { get; set; }
    }
}