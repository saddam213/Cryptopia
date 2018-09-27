
using System.Collections.Generic;

using Newtonsoft.Json;

using Cryptopia.Infrastructure.Incapsula.Common.Classes;

namespace Cryptopia.Infrastructure.Incapsula.Common.Responses
{
  public class DataCenterListResponse : ResponseBase
  {
    [JsonProperty(PropertyName = "DCs")]
    public List<DataCenters> DataCenters { get; set; }
  }
}
