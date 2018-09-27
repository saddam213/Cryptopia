using System.Threading.Tasks;
using Cryptopia.Infrastructure.Common.DataTables;

namespace Cryptopia.Common.User
{
	public interface IUserDepositReader
	{
		Task<DataTablesResponse> GetDataTable(string userId, DataTablesModel model);
	}
}
