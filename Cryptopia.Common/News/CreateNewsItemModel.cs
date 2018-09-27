using System.Web.Mvc;

namespace Cryptopia.Common.News
{
	public class CreateNewsItemModel
	{
		[AllowHtml]
		public string Content { get; set; }
		public string Title { get; set; }
	}
}