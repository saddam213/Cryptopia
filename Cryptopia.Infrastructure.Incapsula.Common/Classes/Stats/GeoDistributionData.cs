using System;
using System.Runtime.Serialization;

namespace Cryptopia.Infrastructure.Incapsula.Common.Classes.Stats
{
    [DataContract]
    public class GeoDistributionData : IDataPoint
    {
        public GeoDistributionData(string location, int number)
        {
            Location = location;
            Number = number;
            Day = new DateTime(1970, 1, 1);
            DateString = Day.ToString("dddd, dd MMMM yyyy hh:mm:ss");
        }

        [DataMember]
        public DateTime Day { get; private set; }

        [DataMember]
        public int Number { get; private set; }

        [DataMember]
        public string DateString { get; private set; }

        [DataMember]
        public string Location { get; set; }
    }
}
