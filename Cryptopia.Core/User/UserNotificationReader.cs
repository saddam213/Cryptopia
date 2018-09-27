using Cryptopia.Infrastructure.Common.DataContext;
using Cryptopia.Common.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;

namespace Cryptopia.Core.User
{
	public class UserNotificationReader : IUserNotificationReader
	{
		public IDataContextFactory DataContextFactory { get; set; }

		public async Task<UserToolbarInfoModel> GetUserToolbarInfo(string userId)
		{
			try
			{
				using (var context = DataContextFactory.CreateReadOnlyContext())
				{
					return await context.Users
						.AsNoTracking()
						.Where(u => u.Id == userId)
						.Select(n => new UserToolbarInfoModel
						{
							MessageCount = n.Messages.Count(x => x.IsInbound && !x.IsRead),
							NotificationCount = n.Notifications.Count(x => !x.Acknowledged)
						}).FirstOrDefaultNoLockAsync().ConfigureAwait(false);
				}
			}
			catch (Exception)
			{
				return new UserToolbarInfoModel();
			}
		}

		public async Task<List<UserNotificationItemModel>> GetUserNotifications(string userId)
		{
			try
			{
				using (var context = DataContextFactory.CreateReadOnlyContext())
				{
					return await context.Notifications
						.AsNoTracking()
						.Where(n => n.UserId == userId)
						.Select(n => new UserNotificationItemModel
						{
							Id = n.Id,
							Type = n.Type,
							Title = n.Title,
							Timestamp = n.Timestamp,
							Notification = n.Notification,
						})
						.OrderByDescending(x => x.Id)
						.Take(1000)
						.ToListNoLockAsync().ConfigureAwait(false);
				}
			}
			catch (Exception)
			{
				return new List<UserNotificationItemModel>();
			}
		}

		public async Task<List<UserNotificationItemModel>> GetUserUnreadNotifications(string userId)
		{
			try
			{
				using (var context = DataContextFactory.CreateReadOnlyContext())
				{
					return await context.Notifications
						.AsNoTracking()
						.Where(n => n.UserId == userId && !n.Acknowledged)
						.Select(n => new UserNotificationItemModel
						{
							Id = n.Id,
							Type = n.Type,
							Title = n.Title,
							Timestamp = n.Timestamp,
							Notification = n.Notification,
						})
						.OrderByDescending(x => x.Id)
						.Take(100)
						.ToListNoLockAsync().ConfigureAwait(false);
				}
			}
			catch (Exception)
			{
				return new List<UserNotificationItemModel>();
			}
		}
	}
}