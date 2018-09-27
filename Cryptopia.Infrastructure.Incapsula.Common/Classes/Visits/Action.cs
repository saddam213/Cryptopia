using Newtonsoft.Json;
using System.Collections.Generic;

namespace Cryptopia.Infrastructure.Incapsula.Common.Classes.Visits
{
  public class Action
  {
    [JsonProperty(PropertyName = "postData")]
    public string PostData { get; set; }

    [JsonProperty(PropertyName = "requestResult")]
    public string RequestResult { get; set; }

    [JsonProperty(PropertyName = "isSecured")]
    public bool IsSecured { get; set; }

    [JsonProperty(PropertyName = "url")]
    public string Url { get; set; }

    [JsonProperty(PropertyName = "httpStatus")]
    public int HttpStatus { get; set; }

    [JsonProperty(PropertyName = "responseTime")]
    public int ResponseTime { get; set; }

    [JsonProperty(PropertyName = "thinkTime")]
    public int ThinkTime { get; set; }

    [JsonProperty(PropertyName = "incidentId")]
    public string IncidentId { get; set; }

    [JsonProperty(PropertyName = "threats")]
    public List<Threat> Threats { get; set; }

    [JsonProperty(PropertyName = "queryString")]
    public string QueryString { get; set; }
  }
}
