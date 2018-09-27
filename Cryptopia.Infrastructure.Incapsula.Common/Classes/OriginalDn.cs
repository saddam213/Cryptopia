using System.Collections.Generic;

using Newtonsoft.Json;

namespace Cryptopia.Infrastructure.Incapsula.Common.Classes
{
  public class OriginalDn
  {
    [JsonProperty(PropertyName = "dns_record_name")]
    public string DNSRecordName { get; set; }

    [JsonProperty(PropertyName = "set_type_to")]
    public string SetTypeTo { get; set; }

    [JsonProperty(PropertyName = "set_data_to")]
    public List<string> SetDataTo { get; set; }
  }
}
