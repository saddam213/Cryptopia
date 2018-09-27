using System.Data.Linq.Mapping;
using System.Runtime.Serialization;
using Cryptopia.Common.Webservice;

namespace Cryptopia.API.Objects
{
	[DataContract]
    [Table(Name = "dbo.Location")]
    public class Location : IIdentity<int>
    {
        [DataMember]
        [Column(AutoSync = AutoSync.OnInsert, DbType = "Int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int Id { get; set; }

        [Column(DbType = "Int NOT NULL")]
        public int ParentId { get; set; }

        [DataMember]
        [Column(DbType = "NVARCHAR(128) NOT NULL")]
        public string Name { get; set; }

        [DataMember]
        [Column(DbType = "NVARCHAR(50) NOT NULL")]
        public string CountryCode { get; set; }
    }
}
