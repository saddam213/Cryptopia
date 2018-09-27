using Newtonsoft.Json;

namespace Cryptopia.Infrastructure.Incapsula.Common.Classes
{
  public class Redirect
  {
    [JsonProperty(PropertyName = "to")]
    public string To { get; set; }

    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; }

    [JsonProperty(PropertyName = "priority")]
    public string Priority { get; set; }

    [JsonProperty(PropertyName = "last_7_days_requests_count")]
    public string LastSevenDaysRequestsCount { get; set; }

    [JsonProperty(PropertyName = "name")]
    public string Name { get; set; }

    [JsonProperty(PropertyName = "action")]
    public string Action { get; set; }

    [JsonProperty(PropertyName = "from")]
    public string From { get; set; }

    [JsonProperty(PropertyName = "filter")]
    public string Filter { get; set; }
  }
}
