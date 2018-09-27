using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptopia.Common.News
{
	public class NewsItemModel
	{
		public string Author { get; set; }
		public string Content { get; set; }
		public DateTime Timestamp { get; set; }
		public string Title { get; set; }
	}
}
