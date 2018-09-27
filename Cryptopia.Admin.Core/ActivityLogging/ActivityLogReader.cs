using Cryptopia.Admin.Common.ActivityLogging;
using Cryptopia.Common.Cache;
using Cryptopia.Infrastructure.Common.DataContext;
using Cryptopia.Infrastructure.Incapsula.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptopia.Admin.Core.ActivityLogging
{
    public class ActivityLogReader : IActivityLogReader
    {
        public ICacheService CacheService { get; set; }
        public IDataContextFactory DataContextFactory { get; set; }

        public async Task<SupportTicketStatsModel> GetSupportTicketStats()
        {
            var cacheResult = await CacheService.GetOrSetMemoryAsync(CacheKey.AdminActivityReports(), TimeSpan.FromMinutes(10), async () =>
            {
                SupportTicketStatsModel model = new SupportTicketStatsModel();

                using (var context = DataContextFactory.CreateReadOnlyContext())
                {
                    DateTime todaysDate = DateTime.UtcNow.Date;

                    var query = await context.SupportTicket
                        .Where(s => s.Status != Enums.SupportTicketStatus.Closed)
                        .ToListNoLockAsync().ConfigureAwait(false);

                    model.TotalOpenTickets = query.Count;

                    query = await context.SupportTicket
                        .Where(s => s.Status != Enums.SupportTicketStatus.Closed && s.Created >= todaysDate)
                        .ToListNoLockAsync().ConfigureAwait(false);

                    model.TotalOpenedToday = query.Count;

                    query = await context.SupportTicket
                        .Where(s => s.Status == Enums.SupportTicketStatus.Closed && s.LastUpdate >= todaysDate)
                        .ToListNoLockAsync().ConfigureAwait(false);

                    model.TotalClosedToday = query.Count;

                    query = await context.SupportTicket
                        .Where(s => s.Status != Enums.SupportTicketStatus.Closed && s.LastUpdate >= todaysDate)
                        .ToListNoLockAsync().ConfigureAwait(false);

                    model.TotalUpdatedToday = query.Count;

                    return model;
                }
            });

            return cacheResult;
        }

        public async Task<VerificationStatsModel> GetVerificationStats()
        {
            var cacheResult = await CacheService.GetOrSetMemoryAsync(CacheKey.AdminActivityReports(), TimeSpan.FromMinutes(10), async () =>
            {
                VerificationStatsModel model = new VerificationStatsModel();
                DateTime todaysDate = DateTime.UtcNow.Date;

                using (var context = DataContextFactory.CreateReadOnlyContext())
                {
                    var query = await context.UserVerification
                        .Where(v => v.User.VerificationLevel == Enums.VerificationLevel.Level2Pending)
                        .ToListNoLockAsync().ConfigureAwait(false);

                    model.TotalAwaitingAction = query.Count;

                    query = await context.UserVerification
                        .Where(v => v.User.VerificationLevel == Enums.VerificationLevel.Level2Pending && v.Timestamp >= todaysDate)
                        .ToListNoLockAsync().ConfigureAwait(false);

                    model.TotalNewToday = query.Count;

                    query = await context.UserVerification
                        .Where(v => v.User.VerificationLevel == Enums.VerificationLevel.Level2 && v.Approved >= todaysDate)
                        .ToListNoLockAsync().ConfigureAwait(false);

                    model.TotalApprovedToday = query.Count;

                    var rejections = await context.UserVerificationReject
                        .Where(v => v.Timestamp >= todaysDate)
                        .ToListNoLockAsync().ConfigureAwait(false);

                    model.TotalRejectedToday = rejections.Count;

                    return model;
                }
            });

            return cacheResult;
        }

        public async Task<UserAdminActivityModel> GetAdminUserActivityStats()
        {
            return await GetActivityStats(DateTime.UtcNow.Date);
        }

        public async Task<ActivityTrendData> GetActivityTrendGraphData(DateTime from)
        {
            var cacheResult = await CacheService.GetOrSetMemoryAsync(CacheKey.AdminActivityReports(), TimeSpan.FromMinutes(10), async () =>
            {
                ActivityTrendData trendData = new ActivityTrendData();

                using (var context = DataContextFactory.CreateReadOnlyContext())
                {
                    var activity = await context.AdminActivityLog
                        .Where(x => x.DateCreated >= from)
                        .OrderBy(x => x.DateCreated)
                        .ToListNoLockAsync().ConfigureAwait(false);

                    var dailyActivity = activity.GroupBy(x => x.DateCreated)
                        .Select(group => new AdminDataModel
                        {
                            DateOccurred = group.Key,
                            Actions = group.Select(x => new AdminAction
                            {
                                ActionDescription = x.ActivityDescription,
                                DateOccurred = x.DateCreated
                            }).ToList()

                        }).ToList();



                    foreach (var dayActivity in dailyActivity)
                    {
                        int openUserVerificationCount = context.UserVerification.Count(v => v.Approved == null && v.Timestamp <= dayActivity.DateOccurred.Date);
                        int openSupportTicketCount = context.SupportTicket.Count(x => x.Status != Enums.SupportTicketStatus.Closed && x.Created <= dayActivity.DateOccurred.Date);

                        trendData.ApprovedVerifications.Add(new ActivityDataPoint(dayActivity.DateOccurred.ToTotalMilliseconds(), dayActivity.VerificationsApproved));
                        trendData.RejectedVerifications.Add(new ActivityDataPoint(dayActivity.DateOccurred.ToTotalMilliseconds(), dayActivity.VerificationsRejected));
                        trendData.OverallVerifications.Add(new ActivityDataPoint(dayActivity.DateOccurred.ToTotalMilliseconds(), openUserVerificationCount));

                        trendData.ClosedSupportTickets.Add(new ActivityDataPoint(dayActivity.DateOccurred.ToTotalMilliseconds(), dayActivity.SupportTicketsClosed));
                        trendData.UpdatedSupportTickets.Add(new ActivityDataPoint(dayActivity.DateOccurred.ToTotalMilliseconds(), dayActivity.SupportTicketsUpdated));
                        trendData.OverallOpenTickets.Add(new ActivityDataPoint(dayActivity.DateOccurred.ToTotalMilliseconds(), openSupportTicketCount));
                    }
                }

                return trendData;
            });

            return cacheResult;
        }

        private async Task<UserAdminActivityModel> GetActivityStats(DateTime from)
        {
            var cacheResult = await CacheService.GetOrSetMemoryAsync(CacheKey.AdminActivityReports(), TimeSpan.FromMinutes(10), async () =>
            {
                UserAdminActivityModel model = new UserAdminActivityModel();

                using (var context = DataContextFactory.CreateReadOnlyContext())
                {
                    var query = await context.AdminActivityLog
                        .Where(x => x.DateCreated >= from)
                        .ToListNoLockAsync().ConfigureAwait(false);

                    model.UserActivity = query.GroupBy(x => x.AdminUserId)
                        .Select(group => new UserAdminModel
                        {
                            UserName = context.Users.FirstOrDefault(u => u.Id == group.Key.ToString()).UserName,
                            Actions = group.Select(x => new AdminAction {
                                ActionDescription = x.ActivityDescription,
                                DateOccurred = x.DateCreated
                            }).ToList()

                        }).ToList();
                }

                return model;
            });

            return cacheResult;
        }
    }
}
