using System.Collections.Generic;

using Newtonsoft.Json;

namespace Cryptopia.Infrastructure.Incapsula.Common.Classes
{
  public class AdvancedCachingRules
  {
    [JsonProperty(PropertyName = "never_cache_resources")]
    public List<object> NeverCacheResources { get; set; }

    [JsonProperty(PropertyName = "always_cache_resources")]
    public List<object> AlwaysCacheResources { get; set; }
  }
}
