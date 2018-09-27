using Cryptopia.Common.News;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cryptopia.Infrastructure.Common.DataContext;
using Cryptopia.Infrastructure.Common.Results;
using Ganss.XSS;

namespace Cryptopia.Core.News
{
	public class NewsWriter : INewsWriter
	{
		public IDataContextFactory DataContextFactory { get; set; }

		public async Task<IWriterResult> CreateNewsItem(string userId, CreateNewsItemModel model)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var sanitizer = new HtmlSanitizer();
				var newsItem = new Entity.NewsItem
				{
					Status = Enums.NewsStatus.Active,
					Timestamp = DateTime.UtcNow,
					Title = model.Title,
					Content = sanitizer.Sanitize(model.Content),
					UserId = userId
				};

				context.NewsItem.Add(newsItem);
				await context.SaveChangesAsync().ConfigureAwait(false);
				return new WriterResult(true, "Successfully created news item");
			}
		}

		public async Task<IWriterResult> UpdateNewsItem(string userId, UpdateNewsItemModel model)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var newsItem = await context.NewsItem.FirstOrDefaultNoLockAsync(x => x.Id == model.Id);
				if (newsItem == null)
					return new WriterResult(false, $"News item #{model.Id} not found.");

				var sanitizer = new HtmlSanitizer();
				newsItem.Title = model.Title;
				newsItem.Content = sanitizer.Sanitize(model.Content);
				newsItem.UserId = userId;
				newsItem.Status = model.Status;
				await context.SaveChangesAsync().ConfigureAwait(false);
				return new WriterResult(true, "Successfully updated news item.");
			}
		}
	}
}
