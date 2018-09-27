using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Cryptopia.Infrastructure.Incapsula.Common.Classes.Stats
{
  [DataContract]
  public class Threat
  {
    [DataMember]
    [JsonProperty(PropertyName = "incidents")]
    public int Incidents { get; set; }

    [DataMember]
    [JsonProperty(PropertyName = "status")]
    public string Status { get; set; }

    [DataMember]
    [JsonProperty(PropertyName = "status_text_id")]
    public string StatusTextId { get; set; }

    [DataMember]
    [JsonProperty(PropertyName = "status_text")]
    public string StatusText { get; set; }

    [DataMember]
    [JsonProperty(PropertyName = "followup")]
    public string FollowUp { get; set; }

    [DataMember]
    [JsonProperty(PropertyName = "followup_text")]
    public string FollowupText { get; set; }

    [DataMember]
    [JsonProperty(PropertyName = "followup_url")]
    public string FollowupUrl { get; set; }

    [DataMember]
    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; }

    [DataMember]
    [JsonProperty(PropertyName = "name")]
    public string Name { get; set; }
  }
}
