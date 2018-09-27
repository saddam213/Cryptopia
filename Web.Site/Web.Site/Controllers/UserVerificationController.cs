using Cryptopia.Common.UserVerification;
using Cryptopia.Enums;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Web.Site.Helpers;

namespace Web.Site.Controllers
{

	public class UserVerificationController : Controller
	{
		public IUserVerificationReader UserVerificationReader { get; set; }
		public IUserVerificationWriter UserVerificationWriter { get; set; }

		[Authorize]
		public async Task<ActionResult> Index()
		{
			return View(await UserVerificationReader.GetVerification(User.Identity.GetUserId()));
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> SubmitVerification(UserVerificationModel model, HttpPostedFileBase identificationImage1, HttpPostedFileBase identificationImage2)
		{
			if (!ModelState.IsValid)
				return View("Index", model);

			if (identificationImage1 == null || identificationImage1.ContentLength == 0 || identificationImage2 == null || identificationImage2.ContentLength == 0)
			{
				var image = identificationImage1 == null || identificationImage1.ContentLength == 0 ? "Identification1" : "Identification2";
				ModelState.AddModelError(image, Resources.User.verificationImageRequiredError);
				return View("Index", model);
			}

			if(!Constant.AllowedImageExtesions.Contains(identificationImage1.ContentType.ToLower()) || !Constant.AllowedImageExtesions.Contains(identificationImage2.ContentType.ToLower()))
			{
				var image = !Constant.AllowedImageExtesions.Contains(identificationImage1.ContentType.ToLower()) ? "Identification1" : "Identification2";
				ModelState.AddModelError(image, Resources.User.verificationImageFormatError);
				return View("Index", model);
			}

			model.Identification1 = identificationImage1.GetBase64String(Constant.VERIFICATION_MAX_WIDTH, Constant.VERIFICATION_MAX_HEIGHT);
			model.Identification2 = identificationImage2.GetBase64String(Constant.VERIFICATION_MAX_WIDTH, Constant.VERIFICATION_MAX_HEIGHT);

			if(string.IsNullOrEmpty(model.Identification1) || string.IsNullOrEmpty(model.Identification1))
			{
				var image = string.IsNullOrEmpty(model.Identification1) ? "Identification1" : "Identification2";
				ModelState.AddModelError(image, Resources.User.verificationImageFormatError);
				return View("Index", model);
			}

			var result = await UserVerificationWriter.CreateVerification(User.Identity.GetUserId(), model);
			if (!ModelState.IsWriterResultValid(result))
				return View("Index", model);


			return RedirectToAction("Index");
		}
	}
}