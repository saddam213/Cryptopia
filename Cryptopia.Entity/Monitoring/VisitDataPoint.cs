using System.ComponentModel.DataAnnotations.Schema;

namespace Cryptopia.Entity
{
    public class VisitDataPoint : DataPoint
    {
        public int VisitSummaryId { get; set; }

        [ForeignKey("VisitSummaryId")]
        public VisitSummary Parent { get; set; }
    }
}
