using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cryptopia.Entity
{
    public class SiteStatistics
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime DateCreated { get; set; }

        public virtual ICollection<VisitSummary> VisitSummaries { get; set; }

        public int? GeoDistributionId { get; set; }

        [ForeignKey("GeoDistributionId")]
        public virtual GeoDistributionSummary GeoDistribution { get; set; }

        public virtual ICollection<VisitDistributionSummary> VisitDistribution { get; set; }

        public int? CachingReportId { get; set; }

        [ForeignKey("CachingReportId")]
        public virtual CachingReport CachingReport { get; set; }

        public virtual ICollection<CachingSummary> CachingSummaries { get; set; }

        public virtual ICollection<HitsSummary> HitsSummaries { get; set; }

        public virtual ICollection<Threat> Threats { get; set; }

        public virtual ICollection<BandwidthSummary> BandwidthSummaries { get; set; }
    }
}
