using System.Linq;
using System.Threading.Tasks;
using Cryptopia.Infrastructure.Common.DataContext;
using Cryptopia.Common.User;
using Cryptopia.Infrastructure.Common.Results;

namespace Cryptopia.Core.User
{
	public class UserWriter : IUserWriter
	{
		public IDataContextFactory DataContextFactory { get; set; }
		public IUserSyncService UserSyncService { get; set; }

		public async Task<IWriterResult> UpdateUser(UserModel model)
		{
			var userId = string.Empty;
			using (var context = DataContextFactory.CreateContext())
			{
				var user = await context.Users
					.Where(x => x.UserName == model.UserName)
					.FirstOrDefaultNoLockAsync().ConfigureAwait(false);
				if (user == null)
					return new WriterResult(false, $"User '{model.UserName}' not found.");

				userId = user.Id;
				model.UserId = userId;
				user.RoleCss = model.RoleCss;
				user.ChatBanEndTime = model.ChatBanEndTime;
				user.ChatTipBanEndTime = model.ChatTipBanEndTime;
				await context.SaveChangesAsync().ConfigureAwait(false);
			}
			await UserSyncService.SyncUser(userId).ConfigureAwait(false);
			return new WriterResult(true, "Successfully updated user.");
		}
	}
}