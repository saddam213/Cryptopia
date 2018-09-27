using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Cryptopia.Infrastructure.Incapsula.Common.Classes.Stats
{
  [DataContract]
  public class Caching
  {
    [DataMember]
    [JsonProperty(PropertyName = "saved_requests")]
    public int SavedRequests { get; set; }

    [DataMember]
    [JsonProperty(PropertyName = "total_requests")]
    public int TotalRequests { get; set; }

    [DataMember]
    [JsonProperty(PropertyName = "saved_bytes")]
    public long SavedBytes { get; set; }

    [DataMember]
    [JsonProperty(PropertyName = "total_bytes")]
    public long TotalBytes { get; set; }
  }
}
