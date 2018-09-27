using System;
using System.Data.Linq.Mapping;

namespace Cryptopia.API.Objects
{
	[Table(Name = "dbo.PoolUser")]
    public class PoolUser
    {
		[Column(DbType = "uniqueidentifier NOT NULL IDENTITY", IsPrimaryKey = true)]
        public Guid Id { get; set; }

		[Column(DbType = "NVARCHAR(50) NOT NULL")]
		public string UserName { get; set; }

		[Column(DbType = "NVARCHAR(20) NOT NULL")]
		public string MiningHandle { get; set; }

		[Column(DbType = "BIT NOT NULL")]
		public bool AllowWorkerNotifications { get; set; }

		[Column(DbType = "BIT NOT NULL")]
		public bool AllowBlockNotifications { get; set; }

		[Column(DbType = "BIT NOT NULL")]
        public bool IsEnabled { get; set; }
	}
}