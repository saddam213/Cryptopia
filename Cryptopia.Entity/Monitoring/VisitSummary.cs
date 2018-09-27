using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cryptopia.Entity
{
    public class VisitSummary : SummaryBase
    {
        public string Name { get; set; }

        public virtual ICollection<VisitDataPoint> Visits { get; set; }
    }
}
