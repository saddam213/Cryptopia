using Cryptopia.Infrastructure.Common.DataTables;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using System.Web.Mvc;
using Cryptopia.Common.Currency;
using Cryptopia.Common.Pool;
using Cryptopia.Common.SiteSettings;
using Web.Site.Extensions;
using Web.Site.Helpers;
using Web.Site.Models;

namespace Web.Site.Controllers
{
	[Authorize(Roles = "Admin, Moderator")]
	public class AdminSettingsController : BaseController
	{
		public ISiteSettingsReader SiteSettingsReader { get; set; }
		public ISiteSettingsWriter SiteSettingsWriter { get; set; }

		[HttpGet]
		[AuthorizeAjax(Roles = "Admin, Moderator")]
		public ActionResult GetSettings()
		{
			return PartialView("_Settings");
		}

		[HttpGet]
		[AuthorizeAjax(Roles = "Admin")]
		public async Task<ActionResult> UpdateSiteSettings()
		{
			var model = await SiteSettingsReader.GetSiteSettings();
			if (model == null)
				return ViewMessageModal(new ViewMessageModel(ViewMessageType.Danger, "Error", "Failed to fetch site settings"));

			return View("UpdateSiteSettingsModal", model);
		}

		[HttpPost]
		[AuthorizeAjax(Roles = "Admin")]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> UpdateSiteSettings(SiteSettingsModel model)
		{
			if (!ModelState.IsValid)
				return View("UpdateSiteSettingsModal", model);

			var result = await SiteSettingsWriter.UpdateSiteSettings(model);
			if (!ModelState.IsWriterResultValid(result))
				return View("UpdateSiteSettingsModal", model);

			return CloseModalSuccess(result.Message);
		}
	}
}