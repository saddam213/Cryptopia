using Cryptopia.Infrastructure.Common.DataContext;
using Cryptopia.Common.User;
using System;
using System.Threading.Tasks;
using Cryptopia.Infrastructure.Common.Results;

namespace Cryptopia.Core.User
{
	public class UserNotificationWriter : IUserNotificationWriter
	{
		public IDataContextFactory DataContextFactory { get; set; }

		public async Task<IWriterResult> Clear(string userId)
		{
			try
			{
				using (var context = DataContextFactory.CreateContext())
				{
					await context.Database.ExecuteSqlCommandAsync("UPDATE UserNotification SET Acknowledged = 1 WHERE UserId = @p0 AND Acknowledged = 0", userId).ConfigureAwait(false);
					return new WriterResult(true);
				}
			}
			catch (Exception)
			{
				return new WriterResult(false);
			}
		}

		public async Task<IWriterResult> Delete(string userId)
		{
			try
			{
				using (var context = DataContextFactory.CreateContext())
				{
					await context.Database.ExecuteSqlCommandAsync("DELETE FROM UserNotification WHERE UserId = @p0", userId).ConfigureAwait(false);
					return new WriterResult(true);
				}
			}
			catch (Exception)
			{
				return new WriterResult(false);
			}
		}
	}
}