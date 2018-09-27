using Newtonsoft.Json;

namespace Cryptopia.Infrastructure.Incapsula.Common.Classes
{
  public class OriginServer
  {
    [JsonProperty(PropertyName = "detected")]
    public bool Detected { get; set; }

    [JsonProperty(PropertyName = "detectionStatus")]
    public string DetectionStatus { get; set; }
  }
}
