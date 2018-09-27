using Newtonsoft.Json;

namespace Cryptopia.Infrastructure.Incapsula.Common.Classes
{
  public class Server
  {
    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; }

    [JsonProperty(PropertyName = "enabled")]
    public string Enabled { get; set; }

    [JsonProperty(PropertyName = "address")]
    public string Address { get; set; }
  }
}
