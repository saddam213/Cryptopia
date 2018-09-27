
using System;
using System.Threading.Tasks;

namespace Cryptopia.Admin.Common.ActivityLogging
{
    public interface IActivityLogReader
    {
        Task<SupportTicketStatsModel> GetSupportTicketStats();

        Task<VerificationStatsModel> GetVerificationStats();

        Task<UserAdminActivityModel> GetAdminUserActivityStats();

        Task<ActivityTrendData> GetActivityTrendGraphData(DateTime from);
    }
}
