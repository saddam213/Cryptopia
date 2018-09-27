using System.Collections.Generic;

using Newtonsoft.Json;

namespace Cryptopia.Infrastructure.Incapsula.Common.Classes
{
  public class LoginProtect
  {
    [JsonProperty(PropertyName = "enabled")]
    public bool Enabled { get; set; }

    [JsonProperty(PropertyName = "specific_users_list")]
    public List<object> SpecificUsersList { get; set; }

    [JsonProperty(PropertyName = "send_lp_notifications")]
    public bool SendLPNotifications { get; set; }

    [JsonProperty(PropertyName = "allow_all_users")]
    public bool AllowAllUsers { get; set; }

    [JsonProperty(PropertyName = "authentication_methods")]
    public List<string> AuthenticationMethods { get; set; }

    [JsonProperty(PropertyName = "urls")]
    public List<object> Urls { get; set; }

    [JsonProperty(PropertyName = "url_patterns")]
    public List<object> UrlPatterns { get; set; }
  }
}
