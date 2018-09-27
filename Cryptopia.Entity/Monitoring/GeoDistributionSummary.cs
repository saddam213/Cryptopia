using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cryptopia.Entity
{
    public class GeoDistributionSummary
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int SiteStatisticsId { get; set; }

        public virtual ICollection<GeoDistributionDataPoint> DistributionData { get; set; }
    }
}
