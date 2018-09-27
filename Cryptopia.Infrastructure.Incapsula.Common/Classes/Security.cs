
using Newtonsoft.Json;

namespace Cryptopia.Infrastructure.Incapsula.Common.Classes
{
  public class Security
  {
    [JsonProperty(PropertyName = "waf")]
    public Waf Waf { get; set; }

    [JsonProperty(PropertyName = "acls")]
    public Acls Acls { get; set; }
  }
}
