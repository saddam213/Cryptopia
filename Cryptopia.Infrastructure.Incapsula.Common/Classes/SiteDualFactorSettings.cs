using System.Collections.Generic;

using Newtonsoft.Json;

namespace Cryptopia.Infrastructure.Incapsula.Common.Classes
{
  public class SiteDualFactorSettings
  {
    [JsonProperty(PropertyName = "enabled")]
    public bool Enabled { get; set; }

    [JsonProperty(PropertyName = "customAreas")]
    public List<object> CustomAreas { get; set; }

    [JsonProperty(PropertyName = "customAreasExceptions")]
    public List<object> CustomAreasExceptions { get; set; }

    [JsonProperty(PropertyName = "allowAllUsers")]
    public bool AllowAllUsers { get; set; }

    [JsonProperty(PropertyName = "shouldSuggestApplicatons")]
    public bool ShouldSuggestApplicatons { get; set; }

    [JsonProperty(PropertyName = "allowedMedia")]
    public List<string> AllowedMedia { get; set; }

    [JsonProperty(PropertyName = "shouldSendLoginNotifications")]
    public bool ShouldSendLoginNotifications { get; set; }

    [JsonProperty(PropertyName = "version")]
    public int Version { get; set; }
  }
}
