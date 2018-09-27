using Cryptopia.Common.News;
using System.Linq;
using System.Threading.Tasks;
using Cryptopia.Infrastructure.Common.DataTables;
using Cryptopia.Infrastructure.Common.DataContext;
using Ganss.XSS;

namespace Cryptopia.Core.News
{
	public class NewsReader : INewsReader
	{
		public IDataContextFactory DataContextFactory { get; set; }

		public async Task<NewsModel> GetNews()
		{
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				var newsData = await context.NewsItem
					.AsNoTracking()
					.Where(x => x.Status == Enums.NewsStatus.Active)
					.Select(x => new NewsItemModel
					{
						Title = x.Title,
						Content = x.Content,
						Author = x.User.UserName,
						Timestamp = x.Timestamp
					})
					.OrderByDescending(x => x.Timestamp)
					.ToListNoLockAsync().ConfigureAwait(false);

				var sanitizer = new HtmlSanitizer();
				foreach (var item in newsData)
				{
					item.Content = sanitizer.Sanitize(item.Content);
				}

				return new NewsModel
				{
					NewsItems = newsData
				};
			}
		}

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