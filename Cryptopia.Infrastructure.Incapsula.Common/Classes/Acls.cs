using System.Collections.Generic;

using Newtonsoft.Json;

namespace Cryptopia.Infrastructure.Incapsula.Common.Classes
{
  public class Acls
  {
    [JsonProperty(PropertyName = "rules")]
    public List<AclRule> Rules { get; set; }
  }
}
