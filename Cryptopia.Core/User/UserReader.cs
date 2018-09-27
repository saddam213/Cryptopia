using System.Linq;
using System.Threading.Tasks;
using Cryptopia.Infrastructure.Common.DataContext;
using Cryptopia.Infrastructure.Common.DataTables;
using Cryptopia.Common.User;
using System.Data.Entity;

namespace Cryptopia.Core.User
{
	public class UserReader : IUserReader
	{
		public IDataContextFactory DataContextFactory { get; set; }

		public async Task<UserModel> GetUserById(string userId)
		{
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				return await context.Users
					.AsNoTracking()
					.Include(x => x.TwoFactor)
					.Where(u => u.Id == userId)
					.Select(user => new UserModel
					{
						UserId = user.Id,
						UserName = user.UserName,
						ChatBanEndTime = user.ChatBanEndTime,
						ChatTipBanEndTime = user.ChatTipBanEndTime,
						RoleCss = user.RoleCss,
						TrustRating = user.TrustRating,
						ShareCount = user.ShareCount,
						KarmaTotal = user.KarmaTotal,
						IsApiEnabled = user.IsApiEnabled,
						IsApiUnsafeWithdrawEnabled = user.IsApiUnsafeWithdrawEnabled,
						IsApiWithdrawEnabled = user.IsApiWithdrawEnabled,
						EmailConfirmed = user.EmailConfirmed,
						DisableLogonEmail = user.DisableLogonEmail,
						DisableRewards = user.DisableRewards,
						DisableTips = user.DisableTips,
						LockoutEnd = user.LockoutEndDateUtc,
						MiningHandle = user.MiningHandle,
						ChatHandle = user.ChatHandle,
						Referrer = user.Referrer,
						Theme = user.Settings.Theme,
						TwoFactor = user.TwoFactor.Select(tfa => string.Concat(tfa.Component, ": ", tfa.Type)).ToList(),
						VerificationLevel = user.VerificationLevel
					}).FirstOrDefaultNoLockAsync().ConfigureAwait(false);
			}
		}

		public async Task<UserModel> GetUserByName(string username)
		{
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				return await context.Users
					.AsNoTracking()
					.Include(x => x.TwoFactor)
					.Where(u => u.UserName == username)
					.Select(user => new UserModel
					{
						UserId = user.Id,
						Email = user.Email,
						UserName = user.UserName,
						ChatBanEndTime = user.ChatBanEndTime,
						ChatTipBanEndTime = user.ChatTipBanEndTime,
						RoleCss = user.RoleCss,
						TrustRating = user.TrustRating,
						ShareCount = user.ShareCount,
						KarmaTotal = user.KarmaTotal,
						IsApiEnabled = user.IsApiEnabled,
						IsApiUnsafeWithdrawEnabled = user.IsApiUnsafeWithdrawEnabled,
						IsApiWithdrawEnabled = user.IsApiWithdrawEnabled,
						EmailConfirmed = user.EmailConfirmed,
						DisableLogonEmail = user.DisableLogonEmail,
						DisableRewards = user.DisableRewards,
						DisableTips = user.DisableTips,
						LockoutEnd = user.LockoutEndDateUtc,
						MiningHandle = user.MiningHandle,
						ChatHandle = user.ChatHandle,
						Referrer = user.Referrer,
						Theme = user.Settings.Theme,
						TwoFactor = user.TwoFactor.Select(tfa => string.Concat(tfa.Component, ": ", tfa.Type)).ToList(),
						VerificationLevel = user.VerificationLevel
					}).FirstOrDefaultNoLockAsync().ConfigureAwait(false);
			}
		}

		public async Task<DataTablesResponse> GetUsers(DataTablesModel model)
		{
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				var query = context.Users
					.AsNoTracking()
					.Where(u => !u.IsDisabled)
					.Select(user => new
					{
						UserName = user.UserName,
						ChatHandle = user.ChatHandle,
						Email = user.Email
					});

				return await query.GetDataTableResultNoLockAsync(model).ConfigureAwait(false);
			}
		}

		public async Task<DataTablesResponse> GetUsersOld(DataTablesModel model)
		{
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				var query = context.Users
					.AsNoTracking()
					.Where(u => !u.IsDisabled)
					.Select(user => new
					{
						UserName = user.UserName,
						ChatHandle = user.ChatHandle,
						MiningHandle = user.MiningHandle,
						Email = user.Email,
						Referrer = user.Referrer,
						Theme = user.Settings.Theme,
						RegisterDate = user.RegisterDate
					});

				return await query.GetDataTableResultNoLockAsync(model).ConfigureAwait(false);
			}
		}
	}
}