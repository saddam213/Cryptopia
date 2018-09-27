using Cryptopia.Admin.Common.News;
using Cryptopia.Common.News;
using Cryptopia.Infrastructure.Common.DataContext;
using Cryptopia.Infrastructure.Common.DataTables;
using Ganss.XSS;
using System.Linq;
using System.Threading.Tasks;

namespace Cryptopia.Admin.Core.News
{
	public class AdminNewsReader : IAdminNewsReader
	{
		public IDataContextFactory DataContextFactory { get; set; }

		public async Task<DataTablesResponse> GetNews(DataTablesModel model)
		{
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				var query = context.NewsItem
					.AsNoTracking()
					.Select(x => new
					{
						Id = x.Id,
						Title = x.Title,
						Author = x.User.UserName,
						Status = x.Status,
						Timestamp = x.Timestamp
					});

				return await query.GetDataTableResultNoLockAsync(model).ConfigureAwait(false);
			}
		}

		public async Task<UpdateNewsItemModel> GetNewsItem(int newsItemId)
		{
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				var result = await context.NewsItem
					.AsNoTracking()
					.Where(x => x.Id == newsItemId)
					.Select(x => new UpdateNewsItemModel
					{
						Id = x.Id,
						Title = x.Title,
						Content = x.Content,
						Status = x.Status,
						Author = x.User.UserName
					}).FirstOrDefaultNoLockAsync().ConfigureAwait(false);
				if (result == null)
					return null;

				var sanitizer = new HtmlSanitizer();
				result.Content = sanitizer.Sanitize(result.Content);
				return result;
			}
		}
	}
}
