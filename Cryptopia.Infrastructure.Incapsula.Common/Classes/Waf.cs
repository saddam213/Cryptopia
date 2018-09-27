using System.Collections.Generic;

using Newtonsoft.Json;

namespace Cryptopia.Infrastructure.Incapsula.Common.Classes
{
  public class Waf
  {
    [JsonProperty(PropertyName = "rules")]
    public List<Rule> Rules { get; set; }
  }
}
