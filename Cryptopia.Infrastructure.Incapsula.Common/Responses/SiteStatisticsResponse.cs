using System.Collections.Generic;

using Newtonsoft.Json;

using Cryptopia.Infrastructure.Incapsula.Common.Classes;
using Cryptopia.Infrastructure.Incapsula.Common.Classes.Stats;

namespace Cryptopia.Infrastructure.Incapsula.Common.Responses
{
  public class SiteStatisticsResponse : ResponseBase
  {
    [JsonProperty(PropertyName = "visits_timeseries")]
    public List<VisitsTimeserie> VisitsTimeseries { get; set; }

    [JsonProperty(PropertyName = "requests_geo_dist_summary")]
    public GeoDistributionSummary RequestsGeoDistributionSummary { get; set; }

    [JsonProperty(PropertyName = "visits_dist_summary")]
    public List<VisitsDistributionSummary> VisitsDistributionSummary { get; set; }

    [JsonProperty(PropertyName = "caching")]
    public Caching Caching { get; set; }

    [JsonProperty(PropertyName = "caching_timeseries")]
    public List<CachingTimeserie> CachingTimeseries { get; set; }

    [JsonProperty(PropertyName = "hits_timeseries")]
    public List<HitsTimeserie> HitsTimeseries { get; set; }

    [JsonProperty(PropertyName = "bandwidth_timeseries")]
    public List<BandwidthTimeserie> BandwidthTimeseries { get; set; }

    [JsonProperty(PropertyName = "threats")]
    public List<Threat> Threats { get; set; }

    [JsonProperty(PropertyName = "incap_rules_timeseries")]
    public List<object> IncapRulesTimeseries { get; set; }

    [JsonProperty(PropertyName = "debug_info")]
    public DebugInfo DebugInfo { get; set; }
  }
}
