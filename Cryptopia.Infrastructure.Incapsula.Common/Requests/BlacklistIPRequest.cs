using System.Collections.Generic;

using Cryptopia.Infrastructure.Incapsula.Common.AppConfiguration;

namespace Cryptopia.Infrastructure.Incapsula.Common.Requests
{
  public class BlacklistIPRequest : RequestBase
  {
    public BlacklistIPRequest(IApplicationConfiguration config, string ip) : base(config)
    {
      IP = ip;
    }

    public string RuleId => "api.acl.blacklisted_ips";

    public override string APITarget => "/sites/configure/acl";

    public override Dictionary<string, string> RequestValues
    {
      get
      {
        var values = base.RequestValues;

        values.Add("site_id", $"{GetSiteId()}");
        values.Add("rule_id", $"{RuleId}");
        values.Add("ips", $"{IP}");

        return values;
      }
    }

    public string IP { get; private set; }
  }
}
