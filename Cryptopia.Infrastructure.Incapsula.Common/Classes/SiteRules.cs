using Newtonsoft.Json;

namespace Cryptopia.Infrastructure.Incapsula.Common.Classes
{
    public class SiteRules
    {
        [JsonProperty(PropertyName = "incap_rules_data")]
        public IncapRulesData IncapRulesData { get; set; }

        [JsonProperty(PropertyName = "ad_rules_data")]
        public AdRulesData AdRulesData { get; set; }
    }
}
