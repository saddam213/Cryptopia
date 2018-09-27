using System.Data.Linq.Mapping;
using System.Runtime.Serialization;
using Cryptopia.Common.Webservice;

namespace Cryptopia.API.Objects
{
	[DataContract]
    [Table(Name = "dbo.MarketItemImage")]
    public class MarketItemImage : IIdentity<int>
    {
        [DataMember]
        [Column(AutoSync = AutoSync.OnInsert, DbType = "INT NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int Id { get; set; }

        [DataMember]
        [Column(DbType = "INT NOT NULL")]
        public int MarketItemId { get; set; }

        [DataMember]
        [Column(DbType = "NVARCHAR(256) NOT NULL")]
        public string Image { get; set; }
    }
}
