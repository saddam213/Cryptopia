using System.ComponentModel.DataAnnotations.Schema;

namespace Cryptopia.Entity
{
    public class CachingReport
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int SiteStatisticsId { get; set; }

        public int SavedRequests { get; set; }

        public int TotalRequests { get; set; }

        public long SavedBytes { get; set; }

        public long TotalBytes { get; set; }

    }
}
