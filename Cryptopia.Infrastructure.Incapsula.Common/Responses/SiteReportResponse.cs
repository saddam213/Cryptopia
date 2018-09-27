using Newtonsoft.Json;

using Cryptopia.Infrastructure.Incapsula.Common.Classes;

namespace Cryptopia.Infrastructure.Incapsula.Common.Responses
{
  public class SiteReportResponse : ResponseBase
  {
    [JsonProperty(PropertyName = "format")]
    public string Format { get; set; }

    [JsonProperty(PropertyName = "report")]
    public string Report { get; set; }

    [JsonProperty(PropertyName = "debug_info")]
    public DebugInfo DebugInfo { get; set; }
  }
}
