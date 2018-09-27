using System.Threading.Tasks;
using Cryptopia.Infrastructure.Common.Results;

namespace Cryptopia.Admin.Common.AdminUser
{
	public interface IAdminUserWriter
	{
		Task<IWriterResult> UpdateUser(AdminUserUpdateModel username);
		Task<IWriterResult> UpdateApi(string adminUserId, AdminUserApiUpdateModel username);
		Task<IWriterResult> UpdateProfile(AdminUserProfileUpdateModel username);
		Task<IWriterResult> UpdateSettings(string adminUserId, AdminUserSettingsUpdateModel username);
		Task<IWriterResult> ChangeEmail(string userId, AdminChangeEmailModel model);
		Task<IWriterResult> ApproveChangeEmail(string userId, AdminChangeEmailApproveModel model);
		Task<IWriterResult> ResetTwoFactor(string userId, AdminResetTwoFactorModel model);
		Task<IWriterResult> ResetAllTwoFactor(string userId, AdminResetTwoFactorModel model);
		Task<IWriterResult> ApproveResetAllTwoFactor(string userId, AdminResetTwoFactorApproveModel model);
		Task<IWriterResult> ActivateUser(string adminUserId, AdminActivateUserModel model);
		Task<IWriterResult> DisableUser(string adminUserId, string userName);
		Task<IWriterResult> UnlockUser(string adminUserId, string userName);
		Task<IWriterResult> LockUser(string adminUserId, string userName);
	}
}
