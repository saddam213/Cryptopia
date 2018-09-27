using System.ComponentModel.DataAnnotations.Schema;

namespace Cryptopia.Entity
{
    public class SummaryBase
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int SiteStatisticsId { get; set; }

        [ForeignKey("SiteStatisticsId")]
        public virtual SiteStatistics SiteStatistics { get; set; }
    }
}
