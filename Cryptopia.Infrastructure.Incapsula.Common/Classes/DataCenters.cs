using System.Collections.Generic;

using Newtonsoft.Json;

namespace Cryptopia.Infrastructure.Incapsula.Common.Classes
{
  public class DataCenters
  {
    [JsonProperty(PropertyName = "isActive")]
    public bool IsActive { get; set; }

    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; }

    [JsonProperty(PropertyName = "enabled")]
    public string Enabled { get; set; }

    [JsonProperty(PropertyName = "servers")]
    public List<Server> Servers { get; set; }

    [JsonProperty(PropertyName = "contentOnly")]
    public string ContentOnly { get; set; }
  }
}
