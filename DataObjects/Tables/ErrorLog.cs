using System;
using System.Data.Linq.Mapping;
using System.Runtime.Serialization;
using Cryptopia.Common.Webservice;

namespace Cryptopia.API.Objects
{
    [DataContract]
    [Table(Name = "dbo.ErrorLog")]
    public class ErrorLog : IIdentity<int>
    {
        [DataMember]
        [Column(AutoSync = AutoSync.OnInsert, DbType = "Int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int Id { get; set; }

        [DataMember]
        [Column(DbType = "nvarchar(128) NOT NULL")]
        public string Component { get; set; }

        [DataMember]
        [Column(DbType = "nvarchar(128) NOT NULL")]
        public string Method { get; set; }

        [DataMember]
        [Column(DbType = "nvarchar(max) NOT NULL")]
        public string Request { get; set; }

        [DataMember]
        [Column(DbType = "nvarchar(max) NOT NULL")]
        public string Exception { get; set; }

        [DataMember]
        [Column(DbType = "DateTime2 NOT NULL")]
        public DateTime Timestamp { get; set; }
    }
}
