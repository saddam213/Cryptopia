using System.Threading.Tasks;
using Cryptopia.Infrastructure.Common.DataTables;

namespace Cryptopia.Common.User
{
	public interface IUserReader
	{
		Task<UserModel> GetUserById(string userId);
		Task<UserModel> GetUserByName(string username);
		Task<DataTablesResponse> GetUsers(DataTablesModel model);
		Task<DataTablesResponse> GetUsersOld(DataTablesModel model);
	}
}