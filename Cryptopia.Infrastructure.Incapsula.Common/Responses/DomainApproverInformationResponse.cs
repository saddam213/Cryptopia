using System.Collections.Generic;

using Newtonsoft.Json;

using Cryptopia.Infrastructure.Incapsula.Common.Classes;

namespace Cryptopia.Infrastructure.Incapsula.Common.Responses
{
  public class DomainApproverInformationResponse : ResponseBase
  {
    [JsonProperty(PropertyName = "domain_emails")]
    public List<string> DomainEmails { get; set; }

    [JsonProperty(PropertyName = "debug_info")]
    public DebugInfo DebugInfo { get; set; }
  }
}
