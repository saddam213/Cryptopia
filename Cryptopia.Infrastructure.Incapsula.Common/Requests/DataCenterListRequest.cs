using System.Collections.Generic;

using Cryptopia.Infrastructure.Incapsula.Common.AppConfiguration;

namespace Cryptopia.Infrastructure.Incapsula.Common.Requests
{
  public class DataCenterListRequest : RequestBase
  {
    public DataCenterListRequest(IApplicationConfiguration config) : base(config)
    { }

    public override string APITarget => "/sites/dataCenters/list";

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
