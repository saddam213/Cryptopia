using Newtonsoft.Json;

namespace Cryptopia.Infrastructure.Incapsula.Common.Classes
{
  public class Rule
  {
    [JsonProperty(PropertyName = "action")]
    public string Action { get; set; }

    [JsonProperty(PropertyName = "action_text")]
    public string ActionText { get; set; }

    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; }

    [JsonProperty(PropertyName = "name")]
    public string Name { get; set; }

    [JsonProperty(PropertyName = "block_bad_bots")]
    public bool? BlockBadBots { get; set; }

    [JsonProperty(PropertyName = "challenge_suspected_bots")]
    public bool? ChallengeSuspectedBots { get; set; }

    [JsonProperty(PropertyName = "activation_mode")]
    public string ActivationMode { get; set; }

    [JsonProperty(PropertyName = "activation_mode_text")]
    public string ActivationModeText { get; set; }

    [JsonProperty(PropertyName = "ddos_traffic_threshold")]
    public int? DDOSTrafficThreshold { get; set; }
  }
}
