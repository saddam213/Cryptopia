using System;

namespace Cryptopia.Infrastructure.Incapsula.Common.Classes.Stats
{
    public class VisitDistributionData : IDataPoint
    {
        public VisitDistributionData(string countryCode, int number)
        {
            CountryCode = countryCode;
            Number = number;
            Day = new DateTime(1970, 1, 1);
            DateString = Day.ToString("dddd, dd MMMM yyyy hh:mm:ss");
        }

        public DateTime Day { get; set; }

        public int Number { get; set; }

        public string DateString { get; set; }

        public string CountryCode { get; set; }
    }
}
