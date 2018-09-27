using Newtonsoft.Json;

namespace Cryptopia.Infrastructure.Incapsula.Common.Classes
{
  public class DebugInfo
  {
    [JsonProperty(PropertyName = "__invalid_name__id-info")]
    public string InvalidNameIdInfo { get; set; }
  }
}
