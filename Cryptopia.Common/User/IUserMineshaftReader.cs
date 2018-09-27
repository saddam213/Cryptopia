using System.Threading.Tasks;
using Cryptopia.Infrastructure.Common.DataTables;
using Cryptopia.Common.Pool;

namespace Cryptopia.Common.User
{
	public interface IUserMineshaftReader
	{
		Task<DataTablesResponse> GetHistory(string userId, DataTablesModel model);
	}
}
