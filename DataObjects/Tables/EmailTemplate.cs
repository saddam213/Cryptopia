using System.Data.Linq.Mapping;
using Cryptopia.Enums;

namespace Cryptopia.API.Objects
{
	[Table(Name = "dbo.EmailTemplate")]
	public class HubEmailTemplate
	{
		[Column(AutoSync = AutoSync.OnInsert, DbType = "Int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
		public int Id { get; set; }

		[Column(DbType = "INT NOT NULL")]
		public EmailTemplateType Type { get; set; }

		[Column(DbType = "nvarchar(256) NOT NULL")]
		public string Subject { get; set; }

		[Column(DbType = "nvarchar(MAX) NOT NULL")]
		public string Template { get; set; }

		[Column(DbType = "BIT NOT NULL")]
		public bool IsEnabled { get; set; }

		[Column(DbType = "BIT NOT NULL")]
		public bool IsHtml { get; set; }
	}
}