using System.Collections.Generic;

using Newtonsoft.Json;

namespace Cryptopia.Infrastructure.Incapsula.Common.Classes
{
  public class IncapRulesData
  {
    [JsonProperty(PropertyName = "All")]
    public List<AllRules> All { get; set; }
  }
}
