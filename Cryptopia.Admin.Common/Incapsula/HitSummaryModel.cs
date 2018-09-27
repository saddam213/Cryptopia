using System.Collections.Generic;

namespace Cryptopia.Admin.Common.Incapsula
{
    public class HitSummaryModel
    {
        public List<HitModel> Hits { get; set; }

        public int Total
        {
            get
            {
                int total = 0;

                if (Hits == null)
                    return total;

                foreach (var v in Hits)
                    total += v.Number;

                return total;
            }
        }
    }
}
