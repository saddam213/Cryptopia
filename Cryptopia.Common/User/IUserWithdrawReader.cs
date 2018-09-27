using System.Threading.Tasks;
using Cryptopia.Infrastructure.Common.DataTables;

namespace Cryptopia.Common.User
{
	public interface IUserWithdrawReader
	{
		Task<DataTablesResponse> GetDataTable(string userId, DataTablesModel model);
	}
}
