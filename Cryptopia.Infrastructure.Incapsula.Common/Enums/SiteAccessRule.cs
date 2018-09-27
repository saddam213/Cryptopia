using System.ComponentModel;

namespace Cryptopia.Infrastructure.Incapsula.Common.Enums
{
  public enum SiteAccessRule
  {
    [Description("api.acl.blacklisted_ips")]
    BlackListedIps = 0,
    [Description("api.acl.blacklisted_urls")]
    BlackListedUrls = 1,
    [Description("api.acl.blacklisted_countries	")]
    BlackListedCountries = 2,
    [Description("api.acl.whitelisted_ips")]
    WhiteListedIps = 3
  }
}
