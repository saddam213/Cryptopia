using System.Threading.Tasks;
using Cryptopia.Infrastructure.Common.DataTables;

namespace Cryptopia.Common.Security
{
	public interface ISecurityReader
	{
		Task<DataTablesResponse> GetUserLogons(DataTablesModel model);
		Task<DataTablesResponse> GetLockedOutUsers(DataTablesModel model);
	}
}