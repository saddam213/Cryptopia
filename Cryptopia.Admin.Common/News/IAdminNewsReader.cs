using Cryptopia.Common.News;
using Cryptopia.Infrastructure.Common.DataTables;
using System.Threading.Tasks;

namespace Cryptopia.Admin.Common.News
{
	public interface IAdminNewsReader
	{
		Task<DataTablesResponse> GetNews(DataTablesModel model);
		Task<UpdateNewsItemModel> GetNewsItem(int newsItemId);
	}
}
