
using System.Collections.Generic;

using Newtonsoft.Json;

using Cryptopia.Infrastructure.Incapsula.Common.Classes;
using Cryptopia.Infrastructure.Incapsula.Common.Classes.Visits;

namespace Cryptopia.Infrastructure.Incapsula.Common.Responses
{
  public class SiteVisitsResponse : ResponseBase
  {
    [JsonProperty(PropertyName = "debug_info")]
    public DebugInfo DebugInfo { get; set; }

    [JsonProperty(PropertyName = "visits")]
    public List<Visit> Visits { get; set; }
  }
}
