
using System.Collections.Generic;

using Cryptopia.Infrastructure.Incapsula.Common.AppConfiguration;

namespace Cryptopia.Infrastructure.Incapsula.Common.Requests
{
  public class SiteStatusRequest : RequestBase
  {
    public SiteStatusRequest(IApplicationConfiguration config) : base(config)
    { }

    public override string APITarget => "/sites/status";

    public override Dictionary<string, string> RequestValues
    {
      get
      {
        var values = base.RequestValues;
        values.Add("site_id", $"{GetSiteId()}");
        return values;
      }
    }

    public string Tests { get; set; }
  }
}
