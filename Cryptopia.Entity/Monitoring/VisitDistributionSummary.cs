using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cryptopia.Entity
{
    public class VisitDistributionSummary : SummaryBase
    {
        public string Name { get; set; }

        public virtual ICollection<VisitDistributionDataPoint> VisitDistibutionData { get; set; }
    }
}
