
using System;
using System.Collections.Generic;

using Cryptopia.Infrastructure.Incapsula.Common.AppConfiguration;
using Cryptopia.Infrastructure.Incapsula.Common.Enums;
using Cryptopia.Infrastructure.Incapsula.Common.Extensions;

namespace Cryptopia.Infrastructure.Incapsula.Common.Requests
{
  public class SiteStatisticsRequest : RequestBase
  {
    public SiteStatisticsRequest(IApplicationConfiguration config, TimeRange timeRange, StatisticsValue statsValues) : base(config)
    {
      TimeRange = timeRange;
      Stats = statsValues;
    }

    public SiteStatisticsRequest(IApplicationConfiguration config, DateTime rangeStart, DateTime rangeEnd, StatisticsValue statsValues) : this(config, TimeRange.Custom, statsValues)
    {
      StartTime = rangeStart.ToTotalMilliseconds();
      EndTime = rangeEnd.ToTotalMilliseconds();
    }

    public override string APITarget => "";

    public override string Endpoint { get { return "/stats/v1"; } }

    public override Dictionary<string, string> RequestValues
    {
      get
      {
        var values = base.RequestValues;
        values.Add("time_range", $"{EnumerationExtensions.GetDescription(TimeRange)}");

        if (TimeRange == TimeRange.Custom)
        {
          values.Add("start", $"{StartTime}");
          values.Add("end", $"{EndTime}");
        }

        values.Add("site_id", $"{GetSiteId()}");

        values.Add("stats", $"{GetStats()}");

        if (Granularity != StatisticGranularity.None)
        {
          if (Granularity == StatisticGranularity.Custom)
            values.Add("granularity", $"{GranularityValue}");
          else
            values.Add("granularity", $"{EnumerationExtensions.GetDescription(Granularity)}");
        }

        return values;
      }
    }

    public TimeRange TimeRange { get; set; }

    public long StartTime { get; set; }

    public long EndTime { get; set; }

    public StatisticsValue Stats { get; set; }

    public StatisticGranularity Granularity { get; set; }

    public long GranularityValue { get; set; }

    #region Private Methods

    private string GetStats()
    {
      string statistics = string.Empty;

      if (Stats.HasFlag(StatisticsValue.All))
        return EnumerationExtensions.GetDescription(StatisticsValue.All);

      if (Stats.HasFlag(StatisticsValue.Visits))
        statistics += EnumerationExtensions.GetDescription(StatisticsValue.Visits);

      if (Stats.HasFlag(StatisticsValue.Hits))
      {
        if (!string.IsNullOrEmpty(statistics))
          statistics += ",";

        statistics += EnumerationExtensions.GetDescription(StatisticsValue.Hits);
      }

      if (Stats.HasFlag(StatisticsValue.Bandwidth))
      {
        if (!string.IsNullOrEmpty(statistics))
          statistics += ",";

        statistics += EnumerationExtensions.GetDescription(StatisticsValue.Bandwidth);
      }

      if (Stats.HasFlag(StatisticsValue.RequestsByDataCenter))
      {
        if (!string.IsNullOrEmpty(statistics))
          statistics += ",";

        statistics += EnumerationExtensions.GetDescription(StatisticsValue.RequestsByDataCenter);
      }

      if (Stats.HasFlag(StatisticsValue.VisitsSummary))
      {
        if (!string.IsNullOrEmpty(statistics))
          statistics += ",";

        statistics += EnumerationExtensions.GetDescription(StatisticsValue.VisitsSummary);
      }

      if (Stats.HasFlag(StatisticsValue.Caching))
      {
        if (!string.IsNullOrEmpty(statistics))
          statistics += ",";

        statistics += EnumerationExtensions.GetDescription(StatisticsValue.Caching);
      }

      if (Stats.HasFlag(StatisticsValue.CachingPerDay))
      {
        if (!string.IsNullOrEmpty(statistics))
          statistics += ",";

        statistics += EnumerationExtensions.GetDescription(StatisticsValue.CachingPerDay);
      }

      if (Stats.HasFlag(StatisticsValue.Threats))
      {
        if (!string.IsNullOrEmpty(statistics))
          statistics += ",";

        statistics += EnumerationExtensions.GetDescription(StatisticsValue.Threats);
      }

      if (Stats.HasFlag(StatisticsValue.IncapRules))
      {
        if (!string.IsNullOrEmpty(statistics))
          statistics += ",";

        statistics += EnumerationExtensions.GetDescription(StatisticsValue.IncapRules);
      }

      if (Stats.HasFlag(StatisticsValue.IncapRulesPerDay))
      {
        if (!string.IsNullOrEmpty(statistics))
          statistics += ",";

        statistics += EnumerationExtensions.GetDescription(StatisticsValue.IncapRulesPerDay);
      }

      return statistics;
    }

    #endregion
  }
}
