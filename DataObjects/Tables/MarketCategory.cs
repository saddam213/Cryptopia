using System.Data.Linq.Mapping;
using System.Runtime.Serialization;
using Cryptopia.Common.Webservice;

namespace Cryptopia.API.Objects
{
	[DataContract]
    [Table(Name = "dbo.MarketCategory")]
    public class MarketCategory : IIdentity<int>
    {
        [DataMember]
        [Column(AutoSync = AutoSync.OnInsert, DbType = "INT NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int Id { get; set; }

        [DataMember]
        [Column(DbType = "INT NOT NULL")]
        public int ParentId { get; set; }

        [DataMember]
        [Column(DbType = "NVARCHAR(128) NOT NULL")]
        public string Name { get; set; }

        [DataMember]
        [Column(DbType = "NVARCHAR(128) NOT NULL")]
        public string DisplayName { get; set; }
    }
}
