using System;
using System.Data.Linq.Mapping;

namespace Cryptopia.API.Objects
{
	[Table(Name = "dbo.UserNotification")]
	public class UserNotification
	{
		[Column(AutoSync = AutoSync.OnInsert, DbType = "INT NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
		public int Id { get; set; }

		[Column(DbType = "NVARCHAR(128) NOT NULL")]
		public string UserId { get; set; }

		[Column(DbType = "NVARCHAR(50) NOT NULL")]
		public string Type { get; set; }

		[Column(DbType = "NVARCHAR(512) NOT NULL")]
		public string Title { get; set; }

		[Column(DbType = "NVARCHAR(2048) NOT NULL")]
		public string Notification { get; set; }

		[Column(DbType = "BIT NOT NULL")]
		public bool Acknowledged { get; set; }

		[Column(DbType = "DATETIME2 NOT NULL")]
		public DateTime Timestamp { get; set; }
	}
}
