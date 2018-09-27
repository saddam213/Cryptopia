using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cryptopia.Entity
{
    public class BandwidthSummary : SummaryBase
    {
        public string Name { get; set; }

        public virtual ICollection<BandwidthDataPoint> BandwidthData { get; set; }
        
    }
}
