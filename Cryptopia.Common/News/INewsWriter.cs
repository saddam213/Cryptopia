using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cryptopia.Infrastructure.Common.Results;

namespace Cryptopia.Common.News
{
	public interface INewsWriter
	{
		Task<IWriterResult> CreateNewsItem(string userId, CreateNewsItemModel model);
		Task<IWriterResult> UpdateNewsItem(string userId, UpdateNewsItemModel model);
	}
}
