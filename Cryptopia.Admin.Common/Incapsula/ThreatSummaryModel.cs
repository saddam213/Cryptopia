using System.Collections.Generic;

namespace Cryptopia.Admin.Common.Incapsula
{
    public class ThreatSummaryModel
    {
        public List<ThreatModel> Threats { get; set; }

        public int Total
        {
            get
            {
                int total = 0;

                if (Threats == null)
                    return total;

                foreach (var v in Threats)
                    total += v.Number;

                return total;
            }
        }
    }
}
