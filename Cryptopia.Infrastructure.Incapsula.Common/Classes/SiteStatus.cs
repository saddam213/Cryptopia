using System;
using System.Collections.Generic;
using System.Linq;

using Cryptopia.Infrastructure.Incapsula.Common.Enums;
using Cryptopia.Infrastructure.Incapsula.Common.Extensions;
using Cryptopia.Infrastructure.Incapsula.Common.Responses;

namespace Cryptopia.Infrastructure.Incapsula.Common.Classes
{
  public class SiteStatus : ResponseData
  {
    public SiteStatus(SiteStatusResponse response) : base(response)
    {
      SiteId = response.SiteId;
      StatusEnum = response.StatusEnum;
      Status = response.Status;
      Domain = response.Domain;
      AccountId = response.AccountId;
      AccelerationLevel = response.AccelerationLevel;
      SiteCreationDate = response.SiteCreationDate.FromTotalMilliseconds();
      IPs = response.IPs;
      DNS = response.DNS;
      OriginalDNS = response.OriginalDNS;
      Warnings = response.Warnings;
      Active = response.Active;
      AdditionalErrors = response.AdditionalErrors;
      DisplayName = response.DisplayName;
      Security = response.Security;
      SealLocation = response.SealLocation;
      SSL = response.SSL;
      SiteDualFactorSettings = response.SiteDualFactorSettings;
      LoginProtect = response.LoginProtect;
      PerformanceConfiguration = response.PerformanceConfiguration;
      ExtendedDDOS = response.ExtendedDDOS;
    }

    public int SiteId { get; set; } 

    public string StatusEnum { get; set; }

    public string Status { get; set; }

    public string Domain { get; set; }

    public int AccountId { get; set; }

    public string AccelerationLevel { get; set; }

    public DateTime SiteCreationDate { get; set; }

    public List<string> IPs { get; set; }

    public List<Dn> DNS { get; set; }

    public List<OriginalDn> OriginalDNS { get; set; }

    public List<object> Warnings { get; set; }

    public string Active { get; set; }

    public List<object> AdditionalErrors { get; set; }

    public string DisplayName { get; set; }

    public Security Security { get; set; }

    public SealLocation SealLocation { get; set; }

    public Ssl SSL { get; set; }

    public SiteDualFactorSettings SiteDualFactorSettings { get; set; }

    public LoginProtect LoginProtect { get; set; }

    public PerformanceConfiguration PerformanceConfiguration { get; set; }

    public int ExtendedDDOS { get; set; }

    public List<string> IpAddressBlacklist
    {
      get
      {
         if (Security != null && Security.Acls != null && Security.Acls.Rules != null)
         {
           var rule = Security.Acls.Rules.FirstOrDefault(r => r.Id == Enums.AclRule.BlacklistedIps.Name());
           return rule.IPs;
         }

         return new List<string>();
      }
    }
  }
}
