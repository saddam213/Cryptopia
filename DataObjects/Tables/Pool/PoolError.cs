using System;
using System.Data.Linq.Mapping;

namespace Cryptopia.API.Objects
{
	[Table(Name = "dbo.PoolError")]
    public class PoolError
    {
        [Column(AutoSync = AutoSync.OnInsert, DbType = "Int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int Id { get; set; }

        [Column(DbType = "nvarchar(20) NOT NULL")]
        public string Pool { get; set; }

        [Column(DbType = "nvarchar(20) NOT NULL")]
        public string Severity { get; set; }

        [Column(DbType = "nvarchar(2048) NOT NULL")]
        public string Message { get; set; }

        [Column(DbType = "nvarchar(MAX) NOT NULL")]
        public string Exception { get; set; }

        [Column(DbType = "DATETIME2 NOT NULL")]
        public DateTime Timestamp { get; set; }
    }

    public enum PoolErrorSeverity
    {
        Low,
        Medium,
        High,
        Urgent
    }
}
