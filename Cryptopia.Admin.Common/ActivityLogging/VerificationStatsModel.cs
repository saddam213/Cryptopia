
namespace Cryptopia.Admin.Common.ActivityLogging
{
    public class VerificationStatsModel
    {
        public int TotalAwaitingAction { get; set; }
        public int TotalNewToday { get; set; }
        public int TotalApprovedToday { get; set; }
        public int TotalRejectedToday { get; set; }
    }
}
