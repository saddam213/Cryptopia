using System.Collections.Generic;

using Newtonsoft.Json;

namespace Cryptopia.Infrastructure.Incapsula.Common.Classes
{
  public class GeneratedCertificate
  {
    [JsonProperty(PropertyName = "san")]
    public List<object> San { get; set; }
  }
}
