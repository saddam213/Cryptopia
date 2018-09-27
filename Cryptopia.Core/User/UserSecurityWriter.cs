using System;
using System.Threading.Tasks;
using Cryptopia.Common.User;
using Cryptopia.Infrastructure.Common.DataContext;
using System.Data.Entity;
using Cryptopia.Common.DataContext;
using Cryptopia.Infrastructure.Common.Results;
using System.Linq;
using Cryptopia.Entity.Auditing;

namespace Cryptopia.Core.User
{
	public class UserSecurityWriter : IUserSecurityWriter
	{
		public IUserSyncService UserSyncService { get; set; }
		public IDataContextFactory DataContextFactory { get; set; }
		public IExchangeDataContextFactory ExchangeDataContextFactory { get; set; }

		public async Task<IWriterResult> UpdateApiSettings(string userId, UpdateApiModel model)
		{
			try
			{
				using (var context = DataContextFactory.CreateContext())
				{
					var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId).ConfigureAwait(false);
					if (user == null)
						return new WriterResult(false, "User not found.");

					model.OldApiKey = user.ApiKey;
					user.IsApiEnabled = model.IsApiEnabled;
					user.IsApiWithdrawEnabled = model.IsApiWithdrawEnabled;
					user.IsApiUnsafeWithdrawEnabled = model.IsApiUnsafeWithdrawEnabled;
					user.ApiKey = model.ApiKey;
					user.ApiSecret = model.ApiSecret;

                    await context.SaveChangesWithAuditAsync().ConfigureAwait(false);
					await UserSyncService.SyncUser(user.Id).ConfigureAwait(false);
					return new WriterResult(true, "Successfully updated API settings.");
				}
			}
			catch (Exception)
			{
				return new WriterResult(false);
			}
		}

		public async Task<IWriterResult> UpdateWithdrawSettings(string userId, UpdateWithdrawModel model)
		{
			try
			{
				using (var context = DataContextFactory.CreateContext())
				{
					var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId).ConfigureAwait(false);
					if (user == null)
						return new WriterResult(false, "User not found.");

					user.DisableWithdrawEmailConfirmation = model.DisableConfirmation;
					user.IsUnsafeWithdrawEnabled = !model.AddressBookOnly;
					await context.SaveChangesWithAuditAsync().ConfigureAwait(false);
					await UserSyncService.SyncUser(user.Id).ConfigureAwait(false);
					return new WriterResult(true, "Successfully updated withdrawal settings.");
				}
			}
			catch (Exception)
			{
				return new WriterResult(false);
			}
		}
	}
}