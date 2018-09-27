using Cryptopia.Common.User;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Web.Site.Controllers
{
	public class UserNotificationController : BaseController
	{
		public IUserNotificationReader UserNotificationReader { get; set; }
		public IUserNotificationWriter UserNotificationWriter { get; set; }

		[HttpGet]
		[Authorize]
		public async Task<ActionResult> GetNotificationCount()
		{
			var toolBarInfo = await UserNotificationReader.GetUserToolbarInfo(User.Identity.GetUserId());
			if (toolBarInfo != null)
			{
				return Json(toolBarInfo, JsonRequestBehavior.AllowGet);
			}
			return Json(new UserToolbarInfoModel(), JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		[Authorize]
		public async Task<ActionResult> GetNotification()
		{
			var notifications = await UserNotificationReader.GetUserNotifications(User.Identity.GetUserId());
			return PartialView("_Notification", new UserNotificationModel
			{
				Notifications = new List<UserNotificationItemModel>(notifications)
			});
		}

		[HttpGet]
		[Authorize]
		public async Task<ActionResult> GetNotificationMenu()
		{
			var notifications = await UserNotificationReader.GetUserUnreadNotifications(User.Identity.GetUserId());
			return Json(notifications, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Delete()
		{
			var result = await UserNotificationWriter.Delete(User.Identity.GetUserId());
			if (result.Success)
				return JsonSuccess();

			return JsonError();
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Clear()
		{
			var result = await UserNotificationWriter.Clear(User.Identity.GetUserId());
			if (result.Success)
				return JsonSuccess();

			return JsonError();
		}
	}
}