using System;
using System.Collections.Generic;

using Cryptopia.Infrastructure.Incapsula.Common.AppConfiguration;
using Cryptopia.Infrastructure.Incapsula.Common.Enums;
using Cryptopia.Infrastructure.Incapsula.Common.Extensions;

namespace Cryptopia.Infrastructure.Incapsula.Common.Requests
{
  public class SiteReportRequest : RequestBase
  {
    public SiteReportRequest(IApplicationConfiguration config, TimeRange range) : base(config)
    {
      TimeRange = range;
    }

    public SiteReportRequest(IApplicationConfiguration config, long startTime, long endTime) : this(config, TimeRange.Custom)
    {
      StartTime = startTime;
      EndTime = endTime;
    }

    public SiteReportRequest(IApplicationConfiguration config, DateTime startTime, DateTime endTime) : this(config, TimeRange.Custom)
    {
      StartTime = startTime.ToTotalMilliseconds();
      EndTime = endTime.ToTotalMilliseconds();
    }

    public override string APITarget => "/sites/report";

    public override Dictionary<string, string> RequestValues
    {
      get
      {
        var values = base.RequestValues;
        values.Add("site_id", $"{GetSiteId()}");
        values.Add("report", $"{Report}");
        values.Add("format", $"{Format}");
        values.Add("time_range", $"{EnumerationExtensions.GetDescription(TimeRange)}");

        if (TimeRange == TimeRange.Custom)
        {
          values.Add("start", $"{StartTime}");
          values.Add("end", $"{EndTime}");
        }

        return values;
      }
    }

    public string Report { get { return "pci-compliance"; } }
    public string Format { get { return "html"; } }
    public TimeRange TimeRange { get; private set; }

    public long StartTime { get; private set; }
    public long EndTime { get; private set; }
  }
}
