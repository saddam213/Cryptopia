using Newtonsoft.Json;

namespace Cryptopia.Infrastructure.Incapsula.Common.Classes.Visits
{
  public class SecuritySummary
  {
    [JsonProperty(PropertyName = "__invalid_name__api.threats.cross_site_scripting")]
    public int NumberOfXSSThreats { get; set; }

    [JsonProperty(PropertyName = "__invalid_name__api.threats.bot_access_control")]
    public int? NumberOfBotAccessControlThreats { get; set; }

    [JsonProperty(PropertyName = "__invalid_name__api.threats.sql_injection")]
    public int? NumberOfSqlInjectionThreats { get; set; }
  }
}
