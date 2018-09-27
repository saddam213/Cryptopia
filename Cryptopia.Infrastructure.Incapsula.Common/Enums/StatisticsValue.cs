
using System;
using System.ComponentModel;

namespace Cryptopia.Infrastructure.Incapsula.Common.Enums
{
  [Flags]
  public enum StatisticsValue
  {
    None = 0x0000,
    [Description("visits_timeseries")]
    Visits = 0x0001,
    [Description("hits_timeseries")]
    Hits = 0x0002,
    [Description("bandwidth_timeseries")]
    Bandwidth = 0x0004,
    [Description("requests_geo_dist_summary")]
    RequestsByDataCenter = 0x0008,
    [Description("visits_dist_summary")]
    VisitsSummary = 0x0016,
    [Description("caching")]
    Caching = 0x0032,
    [Description("caching_timeseries")]
    CachingPerDay = 0x0064,
    [Description("threats")]
    Threats = 0x0128,
    [Description("incap_rules")]
    IncapRules = 0x0256,
    [Description("incap_rules_timeseries")]
    IncapRulesPerDay = 0x0512,
    [Description("visits_timeseries,hits_timeseries,bandwidth_timeseries,requests_geo_dist_summary,visits_dist_summary,caching,caching_timeseries,threats,incap_rules,incap_rules_timeseries")]
    All = 0x094B
  }
}
