using System.Threading.Tasks;
using Cryptopia.Infrastructure.Common.DataTables;

namespace Cryptopia.Admin.Common.AdminUser
{
	public interface IAdminUserReader
	{
		Task<AdminUserDetailsModel> GetUserDetails(string username);
		Task<AdminUserSecurityModel> GetUserSecurity(string username);

		Task<AdminUserUpdateModel> GetUserUpdate(string username);
		Task<AdminUserApiUpdateModel> GetApiUpdate(string username);
		Task<AdminUserProfileUpdateModel> GetProfileUpdate(string username);
		Task<AdminUserSettingsUpdateModel> GetSettingsUpdate(string username);

		Task<DataTablesResponse> GetUserDeposits(string username, DataTablesModel model);
		Task<DataTablesResponse> GetUserWithdraw(string username, DataTablesModel model);
		Task<DataTablesResponse> GetUserTransfer(string username, DataTablesModel model);
		Task<DataTablesResponse> GetUserTrades(string username, DataTablesModel model);
		Task<DataTablesResponse> GetUserTradeHistory(string username, DataTablesModel model);
		Task<DataTablesResponse> GetOpenTickets(string username, DataTablesModel model);
		Task<DataTablesResponse> GetUserBalances(string username, DataTablesModel model);
		Task<DataTablesResponse> GetUserAddresses(string username, DataTablesModel model);
		Task<DataTablesResponse> GetUserLogins(string username, DataTablesModel model);
	}
}