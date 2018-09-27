using Cryptopia.Infrastructure.Common.DataTables;
using System.Threading.Tasks;

namespace Cryptopia.Admin.Common.Referral
{
	public interface IAdminReferralReader
	{
		Task<DataTablesResponse> GetHistory(DataTablesModel model);
	}
}
