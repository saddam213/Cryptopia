using Cryptopia.Infrastructure.Common.DataTables;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using System.Web.Mvc;
using Cryptopia.Common.Deposit;
using Web.Site.Extensions;
using Cryptopia.Common.Emoticons;
using Web.Site.Models;
using Web.Site.Helpers;
using System.Web;

namespace Web.Site.Controllers
{
	public class AdminEmoticonController : BaseController
	{

		public IEmoticonReader EmoticonReader { get; set; }
		public IEmoticonWriter EmoticonWriter { get; set; }

		[HttpGet]
		[AuthorizeAjax(Roles = "Admin, Moderator")]
		public ActionResult GetEmoticons()
		{
			return PartialView("_Emoticons");
		}

		[HttpPost]
		[AuthorizeAjax(Roles = "Admin, Moderator")]
		public async Task<ActionResult> GetEmoticons(DataTablesModel model)
		{
			var emoticonFile = Server.MapPath("~/Content/Images/EmoticonSet/Emoticon.json");
			return DataTable(await EmoticonReader.AdminGetEmoticons(emoticonFile, model));
		}


		[HttpGet]
		[AuthorizeAjax(Roles = "Admin")]
		public async Task<ActionResult> UpdateEmoticon(string code)
		{
			var emoticonFile = Server.MapPath("~/Content/Images/EmoticonSet/Emoticon.json");
			var model = await EmoticonReader.AdminGetEmoticon(emoticonFile, code);
			if (model == null)
				return ViewMessageModal(new ViewMessageModel(ViewMessageType.Danger, "Error", $"Emoticon '{code}' not found"));

			return View("UpdateEmoticonModal", new UpdateEmoticonModel
			{
				Category = model.Category,
				Code = model.Code,
				Name = model.Name
			});
		}

		[HttpPost]
		[AuthorizeAjax(Roles = "Admin")]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> UpdateEmoticon(UpdateEmoticonModel model)
		{
			if (!ModelState.IsValid)
				return View("UpdateEmoticonModal", model);

			var emoticonFile = Server.MapPath("~/Content/Images/EmoticonSet/Emoticon.json");
			var result = await EmoticonWriter.AdminUpdateEmoticon(emoticonFile, model);
			if (!ModelState.IsWriterResultValid(result))
				return View("UpdateEmoticonModal", model);

			return CloseModalSuccess(result.Message);
		}


		[HttpGet]
		[AuthorizeAjax(Roles = "Admin")]
		public ActionResult CreateEmoticon()
		{
			return View("CreateEmoticonModal", new CreateEmoticonModel());
		}

		[HttpPost]
		[AuthorizeAjax(Roles = "Admin")]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> CreateEmoticon(CreateEmoticonModel model)
		{
			if (!ModelState.IsValid)
				return JsonError(ModelState.FirstError());

			var file = Request.Files[0];
			if(file == null)
				return JsonError("Image file is required.");

			model.FileStream = file.InputStream;
			var emoticonFile = Server.MapPath("~/Content/Images/EmoticonSet/Emoticon.json");
			var result = await EmoticonWriter.CreateEmoticon(emoticonFile, model);
			if (!ModelState.IsWriterResultValid(result))
				return JsonError(result.Message);

			return JsonSuccess(result.Message);
		}

		[HttpPost]
		[AuthorizeAjax(Roles = "Admin")]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> DeleteEmoticon(string code)
		{
			var emoticonFile = Server.MapPath("~/Content/Images/EmoticonSet/Emoticon.json");
			var result = await EmoticonWriter.AdminDeleteEmoticon(emoticonFile, new DeleteEmoticonModel
			{
				Code = code
			});
			return Json(result);
		}
	}
}