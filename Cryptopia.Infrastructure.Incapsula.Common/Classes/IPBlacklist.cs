using System.Collections.Generic;
using System.Linq;

using Cryptopia.Infrastructure.Incapsula.Common.Responses;
using Cryptopia.Infrastructure.Incapsula.Common.Extensions;
using System.Runtime.Serialization;

namespace Cryptopia.Infrastructure.Incapsula.Common.Classes
{
  [DataContract]
  public class IPBlacklist : ResponseData
  {
    public IPBlacklist(SiteStatusResponse response) : base(response)
    {
      if (response != null)
      {
        if (response.Security != null)
        {
          if (response.Security.Acls != null && response.Security.Acls.Rules.Any())
          {
            var blockIpRule = response.Security.Acls.Rules.FirstOrDefault(r => r.Id == Enums.AclRule.BlacklistedIps.Name());

            if (blockIpRule != null && blockIpRule.IPs != null)
              BlockedIpAddresses = blockIpRule.IPs;
          }
        }
      }
    }

    [DataMember]
    public List<string> BlockedIpAddresses { get; private set; }
  }
}
