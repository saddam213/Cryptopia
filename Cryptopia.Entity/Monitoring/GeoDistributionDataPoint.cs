using System.ComponentModel.DataAnnotations.Schema;

namespace Cryptopia.Entity
{
    public class GeoDistributionDataPoint : DataPoint
    {
        public int GeoDistributionSummaryId { get; set; }

        [ForeignKey("GeoDistributionSummaryId")]
        public GeoDistributionSummary Parent { get; set; }

        public string Location { get; set; }
    }
}
