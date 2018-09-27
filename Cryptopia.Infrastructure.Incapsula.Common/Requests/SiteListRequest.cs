using Cryptopia.Infrastructure.Incapsula.Common.AppConfiguration;

namespace Cryptopia.Infrastructure.Incapsula.Common.Requests
{
  public class SiteListRequest : RequestBase
  {
    public SiteListRequest(IApplicationConfiguration config) : base(config)
    { }

    public override string APITarget => "/sites/list";
  }
}
