using System;
using System.Collections.Generic;

using Newtonsoft.Json;

using Cryptopia.Infrastructure.Incapsula.Common.Extensions;
using System.Runtime.Serialization;

namespace Cryptopia.Infrastructure.Incapsula.Common.Classes.Stats
{
  [DataContract]
  public class BandwidthTimeserie
  {
    [JsonIgnore]
    private List<BandwidthData> _processedData = null;

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
    public List<BandwidthData> ProcessedData
    {
      get
      {
        if (_processedData == null)
        {
          _processedData = new List<BandwidthData>();

          if (Data == null)
            return _processedData;

          foreach (var dataPoint in Data)
          {
            if (dataPoint.Count == 2)
            {
              string dtString = dataPoint[0];
              string numString = dataPoint[1];

              DateTime dt;
              long milliSeconds = 0;
              if (!long.TryParse(dtString, out milliSeconds))
                continue;

              dt = milliSeconds.FromTotalMilliseconds();

              int number = 0;
              if (!int.TryParse(numString, out number))
                continue;

              _processedData.Add(new BandwidthData(dt, number));
            }
          }
        }

        return _processedData;
      }
    }
  }
}
