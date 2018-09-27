using System.Collections.Generic;

using Newtonsoft.Json;

namespace Cryptopia.Infrastructure.Incapsula.Common.Classes
{
  public class AdRulesData
  {
    [JsonProperty(PropertyName = "Redirect")]
    public List<Redirect> Redirect { get; set; }

    [JsonProperty(PropertyName = "Forward")]
    public List<Forward> Forward { get; set; }
  }
}
