
using System.ComponentModel;

namespace Cryptopia.Infrastructure.Incapsula.Common.Enums
{
  public enum AclRule
  {
    [Description("api.acl.blacklisted_countries")]
    BlacklistedCountries = 0,
    [Description("api.acl.blacklisted_urls")]
    BlacklistedUrls,
    [Description("api.acl.blacklisted_ips")]
    BlacklistedIps,
    [Description("api.acl.whitelisted_ips")]
    WhitelistedIps
  }
}
