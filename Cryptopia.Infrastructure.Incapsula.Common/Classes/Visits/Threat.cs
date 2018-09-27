using Newtonsoft.Json;
using System.Collections.Generic;

namespace Cryptopia.Infrastructure.Incapsula.Common.Classes.Visits
{
  public class Threat
  {
    [JsonProperty(PropertyName = "securityRule")]
    public string SecurityRule { get; set; }

    [JsonProperty(PropertyName = "alertLocation")]
    public string AlertLocation { get; set; }

    [JsonProperty(PropertyName = "threatPattern")]
    public string ThreatPattern { get; set; }

    [JsonProperty(PropertyName = "attackCodes")]
    public List<string> AttackCodes { get; set; }

    [JsonProperty(PropertyName = "securityRuleAction")]
    public string SecurityRuleAction { get; set; }
  }
}
