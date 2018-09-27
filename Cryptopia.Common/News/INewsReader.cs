using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cryptopia.Infrastructure.Common.DataTables;

namespace Cryptopia.Common.News
{
	public interface INewsReader
	{
		Task<NewsModel> GetNews();
		Task<DataTablesResponse> GetNews(DataTablesModel model);
		Task<UpdateNewsItemModel> GetNewsItem(int newsItemId);
	}
}
