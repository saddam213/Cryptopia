using System.Collections.Generic;

namespace Cryptopia.Admin.Common.Incapsula
{
    public class VisitsByCountrySummaryModel
    {
        public List<VisitByCountryModel> VisitsByCountry { get; set; }

        public int Total
        {
            get
            {
                int total = 0;

                if (VisitsByCountry == null)
                    return total;

                foreach (var v in VisitsByCountry)
                    total += v.Number;

                return total;
            }
        }
    }
}
