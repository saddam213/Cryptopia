
using System.Threading.Tasks;

using Cryptopia.Infrastructure.Incapsula.Common.Classes;
using Cryptopia.Infrastructure.Common.DataTables;
using System.Collections.Generic;

namespace Cryptopia.Admin.Common.Incapsula
{
    public interface IIncapsulaReader
    {
        Task<IPBlacklist> GetIPBlacklist();

        Task<VisitSummaryModel> GetSiteVisitsReport();

        Task<HitSummaryModel> GetSiteHitsReport();

        Task<ThreatSummaryModel> GetThreatStatisticsReport();

        Task<VisitsByCountrySummaryModel> GetVisitsByCountryReport();

        Task<GeoDistributionSummaryModel> GetGeoDistribution();
    }
}
