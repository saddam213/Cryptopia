using Cryptopia.Infrastructure.Incapsula.Common.Classes;
using System;
using System.Data.Entity;
using System.Threading.Tasks;

namespace Cryptopia.Common.DataContext
{
    public interface IMonitoringDataContext : IDisposable
    {
        Database Database { get; }
        int SaveChanges();
        Task<int> SaveChangesAsync();

        DbSet<Entity.SiteStatistics> SiteStatisticsReport { get; set; }
        DbSet<Entity.VisitSummary> VisitSummary { get; set; }
        DbSet<Entity.GeoDistributionSummary> GeoDistributionSummary { get; set; }
        DbSet<Entity.VisitDistributionSummary> VisitDistributionSummary { get; set; }

        DbSet<Entity.VisitDistributionDataPoint> VisitDistributionDataPoints { get; set; }
        DbSet<Entity.CachingReport> CachingReport { get; set; }
        DbSet<Entity.CachingSummary> CachingSummary { get; set; }
        DbSet<Entity.HitsSummary> HitsSummary { get; set; }
        DbSet<Entity.BandwidthSummary> BandwidthSummary { get; set; }
        DbSet<Entity.Threat> Threat { get; set; }
    }
}
