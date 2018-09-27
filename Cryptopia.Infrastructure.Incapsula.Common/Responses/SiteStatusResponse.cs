using System.Collections.Generic;

using Newtonsoft.Json;

using Cryptopia.Infrastructure.Incapsula.Common.Classes;

namespace Cryptopia.Infrastructure.Incapsula.Common.Responses
{
  public class SiteStatusResponse : ResponseBase
  {
    [JsonProperty(PropertyName = "site_id")]
    public int SiteId { get; set; }

    [JsonProperty(PropertyName = "statusEnum")]
    public string StatusEnum { get; set; }

    [JsonProperty(PropertyName = "status")]
    public string Status { get; set; }

    [JsonProperty(PropertyName = "domain")]
    public string Domain { get; set; }

    [JsonProperty(PropertyName = "account_id")]
    public int AccountId { get; set; }

    [JsonProperty(PropertyName = "acceleration_level")]
    public string AccelerationLevel { get; set; }

    [JsonProperty(PropertyName = "site_creation_date")]
    public long SiteCreationDate { get; set; }

    [JsonProperty(PropertyName = "ips")]
    public List<string> IPs { get; set; }

    [JsonProperty(PropertyName = "dns")]
    public List<Dn> DNS { get; set; }

    [JsonProperty(PropertyName = "original_dns")]
    public List<OriginalDn> OriginalDNS { get; set; }

    [JsonProperty(PropertyName = "warnings")]
    public List<object> Warnings { get; set; }

    [JsonProperty(PropertyName = "active")]
    public string Active { get; set; }

    [JsonProperty(PropertyName = "additionalErrors")]
    public List<object> AdditionalErrors { get; set; }

    [JsonProperty(PropertyName = "display_name")]
    public string DisplayName { get; set; }

    [JsonProperty(PropertyName = "security")]
    public Security Security { get; set; }

    [JsonProperty(PropertyName = "sealLocation")]
    public SealLocation SealLocation { get; set; }

    [JsonProperty(PropertyName = "ssl")]
    public Ssl SSL { get; set; }

    [JsonProperty(PropertyName = "siteDualFactorSettings")]
    public SiteDualFactorSettings SiteDualFactorSettings { get; set; }

    [JsonProperty(PropertyName = "login_protect")]
    public LoginProtect LoginProtect { get; set; }

    [JsonProperty(PropertyName = "performance_configuration")]
    public PerformanceConfiguration PerformanceConfiguration { get; set; }

    [JsonProperty(PropertyName = "extended_ddos")]
    public int ExtendedDDOS { get; set; }

    [JsonProperty(PropertyName = "debug_info")]
    public DebugInfo DebugInfo { get; set; }
  }
}
