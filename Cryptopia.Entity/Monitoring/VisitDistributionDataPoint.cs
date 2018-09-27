using System.ComponentModel.DataAnnotations.Schema;

namespace Cryptopia.Entity
{
    public class VisitDistributionDataPoint : DataPoint
    {
        public int VisitDistributionSummaryId { get; set; }

        [ForeignKey("VisitDistributionSummaryId")]
        public VisitDistributionSummary Parent { get; set; }

        public string CountryCode { get; set; }
    }
}
