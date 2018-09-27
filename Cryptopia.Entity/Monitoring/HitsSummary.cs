using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cryptopia.Entity
{
    public class HitsSummary : SummaryBase
    {
        public string Name { get; set; }

        public virtual ICollection<HitDataPoint> HitsData { get; set; }
    }
}
