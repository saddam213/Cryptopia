using System.Collections.Generic;

using Newtonsoft.Json;

using Cryptopia.Infrastructure.Incapsula.Common.Classes;

namespace Cryptopia.Infrastructure.Incapsula.Common.Responses
{
  public class SiteListInformationResponse : ResponseBase
  {
    [JsonProperty(PropertyName = "sites")]
    public List<Site> Sites { get; set; }

    [JsonProperty(PropertyName = "debug_info")]
    public DebugInfo DebugInfo { get; set; }
  }
}
