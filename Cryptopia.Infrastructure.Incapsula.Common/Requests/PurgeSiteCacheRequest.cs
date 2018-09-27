using System.Collections.Generic;

using Cryptopia.Infrastructure.Incapsula.Common.AppConfiguration;

namespace Cryptopia.Infrastructure.Incapsula.Common.Requests
{
  public class PurgeSiteCacheRequest : RequestBase
  {
    public PurgeSiteCacheRequest(IApplicationConfiguration config) : base(config)
    { }
    public override string APITarget => "/sites/cache/purge";

    public override Dictionary<string, string> RequestValues
    {
      get
      {
        var values = base.RequestValues;

        values.Add("site_id", $"{GetSiteId()}");
        return values;
      }
    }
  }
}
