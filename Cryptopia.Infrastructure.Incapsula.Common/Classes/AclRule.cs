using System.Collections.Generic;

using Newtonsoft.Json;

namespace Cryptopia.Infrastructure.Incapsula.Common.Classes
{
  public class AclRule
  {
    [JsonProperty(PropertyName = "ips")]
    public List<string> IPs { get; set; }

    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; }

    [JsonProperty(PropertyName = "name")]
    public string Name { get; set; }
  }
}
