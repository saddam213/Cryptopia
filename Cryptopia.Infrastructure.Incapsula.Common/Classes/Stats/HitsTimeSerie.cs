﻿using System;
using System.Collections.Generic;

using Newtonsoft.Json;

using Cryptopia.Infrastructure.Incapsula.Common.Extensions;
using System.Runtime.Serialization;
using System.Linq;

namespace Cryptopia.Infrastructure.Incapsula.Common.Classes.Stats
{
    [DataContract]
    public class HitsTimeserie
    {
        [JsonIgnore]
        private List<HitsData> _processedData = null;

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
        public List<HitsData> ProcessedData
        {
            get
            {
                if (_processedData == null)
                {
                    _processedData = new List<HitsData>();

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

                            _processedData.Add(new HitsData(dt, number));
                        }
                    }
                }

                return _processedData;
            }
        }
    }
}
