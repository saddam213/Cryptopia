using System.Threading.Tasks;
using System.Web.Mvc;
using Web.Site.Models;
using Cryptopia.Enums;
using hbehr.recaptcha;
using Cryptopia.Common.Marketplace;
using Cryptopia.Common.Pool;
using Cryptopia.Infrastructure.Helpers;
using Web.Site.Helpers;

namespace Web.Site.Controllers
{
	public class HomeController : BaseController
	{
		[HttpGet]
		[AllowAnonymous]
		public ActionResult Index()
		{
			return View();
		}

		[HttpGet]
		[AllowAnonymous]
		public ActionResult Terms()
		{
			return View();
		}

		[HttpGet]
		[AllowAnonymous]
		public ActionResult Privacy()
		{
			return View();
		}

		#region Contact Us

		[HttpGet]
		[AllowAnonymous]
		public ActionResult Contact(bool success = false)
		{
			if (success)
				ModelState.AddModelError("Success", Resources.Home.contactSuccessMessage);

			return View(new ContactModel());
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Contact(ContactModel model)
		{
			if (!ModelState.IsValid)
				return View(model);

			if (!CryptopiaAuthenticationHelper.ValidateCaptcha())
			{
				ModelState.AddModelError("", Resources.Home.contactReCaptchaError);
				return View(model);
			}

			if (!await SendSystemEmailAsync(EmailTemplateType.ContactRequest, SystemEmailType.Email_Contact, model.Email, model.Subject, model.Message))
			{
				ModelState.AddModelError("", Resources.Home.contactFailedMessage);
				return View(model);
			}
			ModelState.AddModelError("Success", Resources.Home.contactSuccessMessage);

			return RedirectToAction("Contact", new { success = true });
		}

		#endregion


		[HttpGet]
		[AllowAnonymous]
		public ActionResult Theme()
		{
			return View();
		}

		[HttpGet]
		[AllowAnonymous]
		public ActionResult SetLanguage(string lang)
		{			
			Localization.SetCookie(this.HttpContext.ApplicationInstance.Context, lang);
			
			// return to referrer page or redirect to home
			if (HttpContext.Request.UrlReferrer != null)
				return Redirect(HttpContext.Request.UrlReferrer.ToString());
			else
				return RedirectToAction("");
		}
	}
}