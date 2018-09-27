using System.Collections.Generic;

using Cryptopia.Infrastructure.Incapsula.Common.AppConfiguration;
using Cryptopia.Infrastructure.Incapsula.Common.Constants;

namespace Cryptopia.Infrastructure.Incapsula.Common.Requests
{
  public class DomainAproverEmailAddressesRequest : RequestBase
  {
    public DomainAproverEmailAddressesRequest(IApplicationConfiguration config) : base(config)
    { }

    public override string APITarget { get { return "/domain/emails"; } }
    public override Dictionary<string, string> RequestValues
    {
      get
      {
        var values = base.RequestValues;
        values.Add("domain", GetSiteUrl());
        return values;
      }
    }

    private string GetSiteUrl()
    {
      switch (_config.TargetWebsite)
      {
        case Enums.TargetSite.Cryptopia_Development:
          return StringConstants.appSetting_DevDotCoDotNZ;
        case Enums.TargetSite.Cryptopia_Production:
          return StringConstants.appSetting_ProductionDotCoDotNZ;
        default:
          return string.Empty;
      }
    }
  }
}
