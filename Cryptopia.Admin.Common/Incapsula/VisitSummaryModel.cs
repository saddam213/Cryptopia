using System.Collections.Generic;

namespace Cryptopia.Admin.Common.Incapsula
{
    public class VisitSummaryModel
    {
        public List<VisitModel> Visits { get; set; }

        public int Total
        {
            get
            {
                int total = 0;

                if (Visits == null)
                    return total;

                foreach (var v in Visits)
                    total += v.Number;

                return total;
            }
        }
    }
}
