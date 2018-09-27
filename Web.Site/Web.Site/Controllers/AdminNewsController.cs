using Cryptopia.Infrastructure.Common.DataTables;
using Cryptopia.Common.News;
using Cryptopia.Common.TradePair;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using System.Web.Mvc;
using Web.Site.Extensions;
using Web.Site.Helpers;
using Web.Site.Models;

namespace Web.Site.Controllers
{
	[Authorize(Roles = "Admin, Moderator")]
	public class AdminNewsController : BaseController
	{
		public INewsReader NewsReader { get; set; }
		public INewsWriter NewsWriter { get; set; }

		[HttpGet]
		[AuthorizeAjax(Roles = "Admin")]
		public ActionResult GetNews()
		{
			return PartialView("_News");
		}

		[HttpPost]
		[AuthorizeAjax(Roles = "Admin")]
		public async Task<ActionResult> GetNews(DataTablesModel model)
		{
			return DataTable(await NewsReader.GetNews(model));
		}

		[HttpGet]
		[AuthorizeAjax(Roles = "Admin")]
		public ActionResult CreateNews()
		{
			return View("CreateNewsModal", new CreateNewsItemModel());
		}

		[HttpPost]
		[AuthorizeAjax(Roles = "Admin")]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> CreateNews(CreateNewsItemModel model)
		{
			if (!ModelState.IsValid)
				return View("CreateNewsModal", model);

			var result = await NewsWriter.CreateNewsItem(User.Identity.GetUserId(), model);
			if (!ModelState.IsWriterResultValid(result))
				return View("CreateNewsModal", model);

			return CloseModal(result);
		}


		[HttpGet]
		[AuthorizeAjax(Roles = "Admin")]
		public async Task<ActionResult> UpdateNews(int id)
		{
			var model = await NewsReader.GetNewsItem(id);
			if (model == null)
				return ViewMessageModal(new ViewMessageModel(ViewMessageType.Danger, "Error", $"News item '{id}' not found"));

			return View("UpdateNewsModal", model);
		}

		[HttpPost]
		[AuthorizeAjax(Roles = "Admin")]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> UpdateNews(UpdateNewsItemModel model)
		{
			if (!ModelState.IsValid)
				return View("UpdateNewsModal", model);

			var result = await NewsWriter.UpdateNewsItem(User.Identity.GetUserId(), model);
			if (!ModelState.IsWriterResultValid(result))
				return View("UpdateNewsModal", model);

			return CloseModal(result);
		}

	}
}