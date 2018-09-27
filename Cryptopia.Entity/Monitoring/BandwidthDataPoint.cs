using System.ComponentModel.DataAnnotations.Schema;

namespace Cryptopia.Entity
{
    public class BandwidthDataPoint : DataPoint
    {
        public int BandwidthSummaryId { get; set; }

        [ForeignKey("BandwidthSummaryId")]
        public BandwidthSummary Parent { get; set; }
    }
}
