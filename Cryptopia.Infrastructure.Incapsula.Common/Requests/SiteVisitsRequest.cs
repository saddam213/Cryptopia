
using System;
using System.Collections.Generic;

using Cryptopia.Infrastructure.Incapsula.Common.Enums;
using Cryptopia.Infrastructure.Incapsula.Common.Extensions;
using Cryptopia.Infrastructure.Incapsula.Common.AppConfiguration;

namespace Cryptopia.Infrastructure.Incapsula.Common.Requests
{
  public class SiteVisitsRequest : RequestBase
  {
    public SiteVisitsRequest(IApplicationConfiguration config, TimeRange range) : base(config)
    {
      TimeRange = range;
    }

    public SiteVisitsRequest(IApplicationConfiguration config, DateTime rangeStart, DateTime rangeEnd) : this(config, TimeRange.Custom)
    {
      StartTime = rangeStart.ToTotalMilliseconds();
      EndTime = rangeEnd.ToTotalMilliseconds();
    }

    public override string APITarget => "";

    public override string Endpoint { get { return "/visits/v1"; } }

    public override Dictionary<string, string> RequestValues
    {
      get
      {
        var values = base.RequestValues;

        if (TimeRange == TimeRange.None)
          TimeRange = TimeRange.Today;

        values.Add("time_range", $"{EnumerationExtensions.GetDescription(TimeRange)}");

        if (TimeRange == TimeRange.Custom)
        {
          values.Add("start", $"{StartTime}");
          values.Add("end", $"{EndTime}");
        }

        values.Add("site_id", $"{GetSiteId()}");

        return values;
      }
    }

    public TimeRange TimeRange { get; set; }
    public long StartTime { get; set; }
    public long EndTime { get; set; }
    public int PageSize { get; set; }
    public int PageNum { get; set; }
    public string Security { get; set; }
    public string Country { get; set; }
    public string IP { get; set; }
    public string VisitIds { get; set; }
    public bool ShowLiveVisits { get; set; }
  }
}
