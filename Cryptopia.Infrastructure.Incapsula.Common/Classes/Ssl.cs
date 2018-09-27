using Newtonsoft.Json;

namespace Cryptopia.Infrastructure.Incapsula.Common.Classes
{
  public class Ssl
  {
    [JsonProperty(PropertyName = "origin_server")]
    public OriginServer OriginServer { get; set; }

    [JsonProperty(PropertyName = "generated_certificate")]
    public GeneratedCertificate GeneratedCertificate { get; set; }
  }
}
