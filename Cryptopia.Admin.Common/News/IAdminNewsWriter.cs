using Cryptopia.Common.News;
using Cryptopia.Infrastructure.Common.Results;
using System.Threading.Tasks;

namespace Cryptopia.Admin.Common.News
{
	public interface IAdminNewsWriter
	{
		Task<IWriterResult> CreateNewsItem(string userId, CreateNewsItemModel model);
		Task<IWriterResult> UpdateNewsItem(string userId, UpdateNewsItemModel model);
	}
}
