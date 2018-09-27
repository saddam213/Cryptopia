using Cryptopia.Infrastructure.Common.DataTables;
using System.Threading.Tasks;

namespace Cryptopia.Common.Shareholder
{
	public interface IShareholderReader
	{
		Task<ShareholderModel> GetShareInfo(string userId);
		Task<DataTablesResponse> GetPayoutHistory(string userId, DataTablesModel model);
		Task<DataTablesResponse> GetPaytopiaHistory(DataTablesModel model);
	}
}
