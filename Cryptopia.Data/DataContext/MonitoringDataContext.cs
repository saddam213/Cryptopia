using System;
using System.Data.Entity;
using Cryptopia.Common.DataContext;
using Cryptopia.Entity;
using System.Diagnostics;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Cryptopia.Data.DataContext
{
    public class MonitoringDataContext : DbContext, IMonitoringDataContext
    {
        public MonitoringDataContext() : base("DefaultMonitoringConnection")
        {
            Database.Log = (e) => Debug.WriteLine(e);
        }

        public virtual DbSet<SiteStatistics> SiteStatisticsReport { get; set; }
        public virtual DbSet<VisitSummary> VisitSummary { get; set; }
        public virtual DbSet<GeoDistributionSummary> GeoDistributionSummary { get; set; }
        public virtual DbSet<VisitDistributionSummary> VisitDistributionSummary { get; set; }
        public virtual DbSet<VisitDistributionDataPoint> VisitDistributionDataPoints { get; set; }
        public virtual DbSet<CachingReport> CachingReport { get; set; }
        public virtual DbSet<CachingSummary> CachingSummary { get; set; }
        public virtual DbSet<HitsSummary> HitsSummary { get; set; }
        public virtual DbSet<BandwidthSummary> BandwidthSummary { get; set; }
        public virtual DbSet<Threat> Threat { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            try
            {
                modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
                modelBuilder.Entity<SiteStatistics>().HasOptional(s => s.CachingReport);
                modelBuilder.Entity<SiteStatistics>().HasOptional(s => s.GeoDistribution);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
