using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;
using Cryptopia.Admin.Common.Incapsula;
using Cryptopia.Admin.Core.AdmintopiaService;
using Cryptopia.Infrastructure.Common.DataTables;
using Cryptopia.Infrastructure.Incapsula.Common.Classes;
using Cryptopia.Common.DataContext;
using Cryptopia.Infrastructure.Common.DataContext;
using System.Collections.Generic;
using System;

namespace Cryptopia.Admin.Core.Incapsula
{
    public class IncapsulaReader : IIncapsulaReader
    {
        public IMonitoringDataContextFactory MonitoringDataContextFactory { get; set; }

        public async Task<IPBlacklist> GetIPBlacklist()
        {
            using (var service = new AdmintopiaServiceClient())
            {
                return await service.GetIpAddressBlacklistAsync();
            }
        }

        public async Task<VisitSummaryModel> GetSiteVisitsReport()
        {
            using (var context = MonitoringDataContextFactory.CreateReadOnlyContext())
            {
                var report = await context.SiteStatisticsReport
                    .AsNoTracking()
                    .OrderByDescending(ss => ss.DateCreated).FirstOrDefaultNoLockAsync().ConfigureAwait(false);

				if (report == null)
				{
					return null;
				}

                return new VisitSummaryModel()
                {
                    Visits = await context.VisitSummary
                    .AsNoTracking()
                    .Where(v => v.SiteStatisticsId == report.Id)
                    .OrderBy(v => v.Id)
                    .Select(vs => new VisitModel
                    {
                        Name = vs.Name,
                        Number = vs.Visits.Sum(vdp => vdp.NumberOccurred)
                    }).ToListNoLockAsync().ConfigureAwait(false)
                };

            }
        }

        public async Task<HitSummaryModel> GetSiteHitsReport()
        {
            using (var context = MonitoringDataContextFactory.CreateReadOnlyContext())
            {
                var report = await context.SiteStatisticsReport
                    .AsNoTracking()
                    .OrderByDescending(ss => ss.DateCreated).FirstOrDefaultNoLockAsync().ConfigureAwait(false);

				if (report == null)
				{
					return null;
				}

				return new HitSummaryModel()
                {
                    Hits = await context.HitsSummary
                    .AsNoTracking()
                    .Where(v => v.SiteStatisticsId == report.Id)
                    .OrderBy(v => v.Id)
                    .Select(hs => new HitModel
                    {
                        Name = hs.Name,
                        Number = hs.HitsData.Sum(vdp => vdp.NumberOccurred)
                    }).ToListNoLockAsync().ConfigureAwait(false)
                };

            }
        }

        public async Task<ThreatSummaryModel> GetThreatStatisticsReport()
        {
            using (var context = MonitoringDataContextFactory.CreateReadOnlyContext())
            {
                var report = await context.SiteStatisticsReport
                    .AsNoTracking()
                    .OrderByDescending(ss => ss.DateCreated).FirstOrDefaultNoLockAsync().ConfigureAwait(false);

				if (report == null)
				{
					return null;
				}

				return new ThreatSummaryModel()
                {
                    Threats = await context.Threat
                    .AsNoTracking()
                    .Where(v => v.SiteStatisticsId == report.Id && v.Incidents >= 0)
                    .GroupBy(g => g.Name)
                    .Select(group => new ThreatModel
                    {
                        Name = group.Key,
                        Number = group.Sum(a => a.Incidents),
                        Status = group.FirstOrDefault().Status,
                        StatusText = group.FirstOrDefault().StatusText
                    }).ToListNoLockAsync().ConfigureAwait(false)
                };

            }
        }

        public async Task<VisitsByCountrySummaryModel> GetVisitsByCountryReport()
        {
            using (var context = MonitoringDataContextFactory.CreateReadOnlyContext())
            {
                var report = await context.SiteStatisticsReport
                    .AsNoTracking()
                    .OrderByDescending(ss => ss.DateCreated).FirstOrDefaultNoLockAsync().ConfigureAwait(false);

				if (report == null)
				{
					return null;
				}

				return await context.VisitDistributionSummary
                    .AsNoTracking()
                    .Where(s => s.SiteStatisticsId == report.Id && s.Name == "Visits by country")
                    .Select(x => new VisitsByCountrySummaryModel
                    {
                        VisitsByCountry = x.VisitDistibutionData
                        .Select(y => new VisitByCountryModel
                        {
                            CountryCode = y.CountryCode,
                            Number = y.NumberOccurred
                        }).ToList()
                    }).FirstOrDefaultNoLockAsync().ConfigureAwait(false);
            }
        }

        public async Task<GeoDistributionSummaryModel> GetGeoDistribution()
        {
            using (var context = MonitoringDataContextFactory.CreateReadOnlyContext())
            {
                var report = await context.SiteStatisticsReport
                    .AsNoTracking()
                    .OrderByDescending(ss => ss.DateCreated).FirstOrDefaultNoLockAsync();

				if (report == null)
				{
					return null;
				}

				var summary = await context.GeoDistributionSummary
                    .AsNoTracking()
                    .Include(i => i.DistributionData)
                    .Where(v => v.SiteStatisticsId == report.Id)
                    .FirstOrDefaultNoLockAsync();

				if (summary == null)
				{
					return null;
				}

				return new GeoDistributionSummaryModel
                {
                    Distribution = summary.DistributionData
                    .Select(v => new GeoDistributionModel
                    {
                        Location = v.Location,
                        Number = v.NumberOccurred
                    }).ToList()
                };
            }
        }
    }
}
