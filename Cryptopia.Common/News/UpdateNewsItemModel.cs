using Cryptopia.Enums;
using System.Web.Mvc;

namespace Cryptopia.Common.News
{
	public class UpdateNewsItemModel
	{
		public string Author { get; set; }

		[AllowHtml]
		public string Content { get; set; }
		public int Id { get; set; }
		public NewsStatus Status { get; set; }
		public string Title { get; set; }
	}
}