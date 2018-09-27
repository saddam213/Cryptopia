using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using Newtonsoft.Json;

using Cryptopia.Infrastructure.Incapsula.Common.Extensions;

namespace Cryptopia.Infrastructure.Incapsula.Common.Classes.Stats
{
  [DataContract]
  public class GeoDistributionSummary
  {
    [JsonIgnore]
    private List<GeoDistributionData> _processedData = null;

    [JsonProperty(PropertyName = "data")]
    public List<List<string>> Data { get; set; }

    [DataMember]
    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; }

    [DataMember]
    [JsonProperty(PropertyName = "name")]
    public string Name { get; set; }

    [DataMember]
    [JsonIgnore]
    public List<GeoDistributionData> ProcessedData
    {
      get
      {
        if (_processedData == null)
        {
          _processedData = new List<GeoDistributionData>();

          if (Data == null)
            return _processedData;

          foreach (var dataPoint in Data)
          {
            if (dataPoint.Count == 2)
            {
              string locationString = dataPoint[0];
              string numString = dataPoint[1];

              int number = 0;
              if (!int.TryParse(numString, out number))
                continue;

              _processedData.Add(new GeoDistributionData(locationString, number));
            }
          }
        }

        return _processedData;
      }
    }
  }
}
