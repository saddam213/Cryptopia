using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cryptopia.Entity
{
    public class CachingSummary : SummaryBase
    {
        public string Name { get; set; }

        public virtual ICollection<CachingDataPoint> CachingData { get; set; }
    }
}
