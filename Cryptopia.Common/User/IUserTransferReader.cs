using Cryptopia.Infrastructure.Common.DataTables;
using System.Threading.Tasks;

namespace Cryptopia.Common.User
{
	public interface IUserTransferReader
	{
		Task<DataTablesResponse> GetDataTable(string userId, DataTablesModel model);
	}
}