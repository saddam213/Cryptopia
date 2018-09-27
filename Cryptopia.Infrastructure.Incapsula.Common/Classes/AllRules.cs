using Newtonsoft.Json;

namespace Cryptopia.Infrastructure.Incapsula.Common.Classes
{
  public class AllRules
  {
    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; }

    [JsonProperty(PropertyName = "last_7_days_requests_count")]
    public string LastSevenDaysRequestsCount { get; set; }

    [JsonProperty(PropertyName = "name")]
    public string Name { get; set; }

    [JsonProperty(PropertyName = "action")]
    public string Action { get; set; }

    [JsonProperty(PropertyName = "filter")]
    public string Filter { get; set; }
  }
}
