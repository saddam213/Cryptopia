using Cryptopia.Common.Webservice;
using System.Data.Linq.Mapping;

namespace Cryptopia.API.Objects
{
	[Table(Name = "dbo.IntegrationExchange")]
	public class IntegrationExchange : IIdentity<int>
	{
		[Column(AutoSync = AutoSync.OnInsert, DbType = "Int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
		public int Id { get; set; }

		[Column(DbType = "NVARCHAR(50) NOT NULL")]
		public string Name { get; set; }


		[Column(DbType = "INT NOT NULL")]
		public int Order { get; set; }

		[Column(DbType = "bit NOT NULL")]
		public bool IsEnabled { get; set; }
	}
}
