using Cryptopia.Infrastructure.Common.DataTables;
using System.Threading.Tasks;

namespace Cryptopia.Admin.Common.Security
{
	public interface IAdminSecurityReader
	{
		Task<DataTablesResponse> GetLockedOutUsers(DataTablesModel model);
	}
}
