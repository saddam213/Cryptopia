using Cryptopia.Common.User;
using Cryptopia.Enums;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using System.Web.Mvc;
using Web.Site.Helpers;
using Web.Site.Notifications;

namespace Web.Site.Controllers
{
	public class UserSettingsController : BaseController
	{
		public IUserSettingsReader UserSettingsReader { get; set; }
		public IUserSettingsWriter UserSettingsWriter { get; set; }
		
		[HttpGet]
		[Authorize]
		public async Task<ActionResult> GetSettings()
		{
			var settings = await UserSettingsReader.GetSettings(User.Identity.GetUserId());
			if (settings != null)
			{
				return PartialView("_Settings", settings);
			}
			return PartialView("_Settings", new UserSettingsModel());
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> UpdateSettings(UserSettingsModel model)
		{
			if (!ModelState.IsValid)
			{
				return PartialView("_Settings", model);
			}

			var result = await UserSettingsWriter.UpdateSettings(User.Identity.GetUserId(), model);
			if (!ModelState.IsWriterResultValid(result))
			{
				return PartialView("_Settings", model);
			}

			User.UpdateClaim(CryptopiaClaim.Theme, model.Theme.ToString());
			await ChatHub.InvalidateUserCache(User.Identity.GetUserId());
			return PartialView("_Settings", model);
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> UpdateTheme()
		{
			var theme = ClaimsUser.Theme == "Dark"
				? SiteTheme.Light
				: SiteTheme.Dark;

			var result = await UserSettingsWriter.UpdateTheme(User.Identity.GetUserId(), theme);
			if (!result.Success)
				return JsonError(ClaimsUser.Theme);

			User.UpdateClaim(CryptopiaClaim.Theme, theme.ToString());
			return JsonSuccess(theme.ToString());
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> SubmitChatIgnore(string username)
		{
			var result = await UserSettingsWriter.UpdateIgnoreList(User.Identity.GetUserId(), UserIgnoreListType.Chat, username);
			if(result.Success)
				await ChatHub.InvalidateUserCache(User.Identity.GetUserId());

			return Json(new
			{
				Success = result.Success,
				Message = result.Message,
				WasRemoved = result.Result
			});
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> SubmitTipIgnore(string username)
		{
			var result = await UserSettingsWriter.UpdateIgnoreList(User.Identity.GetUserId(), UserIgnoreListType.Tip, username);
			if (result.Success)
				await ChatHub.InvalidateUserCache(User.Identity.GetUserId());

			return Json(new
			{
				Success = result.Success,
				Message = result.Message,
				WasRemoved = result.Result
			});
		}

		[HttpPost]
		[Authorize]
		public async Task<ActionResult> UpdateChartSettings(string settings)
		{
			var result = await UserSettingsWriter.UpdateChartSettings(User.Identity.GetUserId(), settings);
			if (!result.Success)
				return JsonError(result.Message);

			User.UpdateClaim(CryptopiaClaim.ChartSettings, settings);
			return JsonSuccess(result.Message);
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> UpdateBalanceHideZero(bool hide)
		{
			var result = await UserSettingsWriter.UpdateBalanceHideZero(User.Identity.GetUserId(), hide);
			if (!result.Success)
				return JsonError();

			User.UpdateClaim(CryptopiaClaim.HideZeroBalance, hide.ToString());
			return JsonSuccess();
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> UpdateBalanceFavoritesOnly(bool show)
		{
			var result = await UserSettingsWriter.UpdateBalanceFavoriteOnly(User.Identity.GetUserId(), show);
			if (!result.Success)
				return JsonError();

			User.UpdateClaim(CryptopiaClaim.ShowFavoriteBalance, show.ToString());
			return JsonSuccess();
		}

	}
}