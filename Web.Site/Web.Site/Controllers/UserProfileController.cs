using Cryptopia.Common.Image;
using Cryptopia.Common.Referral;
using Cryptopia.Common.User;
using Cryptopia.Enums;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

using Web.Site.Helpers;
using Web.Site.Models;
using Web.Site.Notifications;

namespace Web.Site.Controllers
{
	public class UserProfileController : BaseController
	{
		public IImageService ImageService { get; set; }
		public IUserProfileReader UserProfileReader { get; set; }
		public IUserProfileWriter UserProfileWriter { get; set; }


		[HttpGet]
		[Authorize]
		public async Task<ActionResult> GetProfile()
		{
			var model = await UserProfileReader.GetProfile(User.Identity.GetUserId());
			if (model != null)
			{
				return PartialView("_Profile", model);
			}
			return PartialView("_Profile", new UserProfileModel());
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> UpdateProfile(UserProfileModel model)
		{
			if (!ModelState.IsValid)
			{
				return PartialView("_Profile", model);
			}

			var result = await UserProfileWriter.UpdateProfile(User.Identity.GetUserId(), model);
			if (!ModelState.IsWriterResultValid(result))
			{
				return PartialView("_Profile", model);
			}

			User.UpdateClaim(CryptopiaClaim.ChatHandle, model.ChatHandle);
			await ChatHub.InvalidateUserCache(User.Identity.GetUserId());
			return PartialView("_Profile", model);
		}

		[HttpGet]
		[Authorize]
		public ActionResult UpdateAvatar()
		{
			return View("UpdateAvatarModal");
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> UpdateAvatar(HttpPostedFileBase AvatarFile)
		{
			var result = await ImageService.SaveImage(new CreateImageModel
			{
				Name = User.Identity.GetUserName(),
				Directory = HostingEnvironment.MapPath("~/Content/Images/Avatar"),
				FileStream = AvatarFile.InputStream,
				MaxHeight = 60,
				MaxWidth = 60,
				MaxFileSize = 200 * 1024,
				CanResize = true
			});
			if (!result.Success)
				return ViewMessage(new ViewMessageModel(ViewMessageType.Danger, Resources.User.profileAvatarErrorTitle, result.Message, showContinueLink: true, continueLink: Url.RouteUrl("Account"), continueLinkText: Resources.User.profileAvatarErrorLink));

			await ChatHub.InvalidateUserCache(User.Identity.GetUserId());
			return RedirectToRoute("Account");
		}
	}
}