using System;
using System.Runtime.Serialization;

namespace Cryptopia.Infrastructure.Incapsula.Common.Classes.Stats
{
    [DataContract]
    public class VisitData : IDataPoint
    {
        public VisitData(DateTime date, int number)
        {
            Day = date;
            Number = number;
            DateString = Day.ToString("dddd, dd MMMM yyyy hh:mm:ss");
        }

        [DataMember]
        public DateTime Day { get; private set; }

        [DataMember]
        public int Number { get; private set; }

        [DataMember]
        public string DateString { get; private set; }
    }
}
