using System.ComponentModel.DataAnnotations.Schema;

namespace Cryptopia.Entity
{
    public class HitDataPoint : DataPoint
    {
        public int HitSummaryId { get; set; }

        [ForeignKey("HitSummaryId")]
        public HitsSummary Parent { get; set; }
    }
}
