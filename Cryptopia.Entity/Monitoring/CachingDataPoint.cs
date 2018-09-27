
using System.ComponentModel.DataAnnotations.Schema;

namespace Cryptopia.Entity
{
    public class CachingDataPoint : DataPoint
    {
        public int CachingSummaryId { get; set; }

        [ForeignKey("CachingSummaryId")]
        public CachingSummary Parent { get; set; }
    }
}
