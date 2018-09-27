using Cryptopia.Infrastructure.Common.DataTables;
using System.Threading.Tasks;
using System.Web.Mvc;
using Cryptopia.Admin.Common.AdminUser;
using Cryptopia.Common.User;
using Web.Site.Extensions;
using Web.Site.Helpers;
using Web.Site.Models;
using Web.Site.Notifications;
using Microsoft.AspNet.Identity;

namespace Web.Site.Controllers
{
	public class AdminUserController : BaseController
	{
		public IUserReader UserReader { get; set; }
		public IUserWriter UserWriter { get; set; }
		public IAdminUserReader AdminUserReader { get; set; }
		public IAdminUserWriter AdminUserWriter { get; set; }

		[HttpGet]
		[AuthorizeAjax(Roles = "Admin, Moderator")]
		public ActionResult GetUserAdmin()
		{
			return PartialView("_UserAdmin");
		}

		[HttpPost]
		[AuthorizeAjax(Roles = "Admin, Moderator")]
		public async Task<ActionResult> GetUsers(DataTablesModel model)
		{
			return DataTable(await UserReader.GetUsersOld(model));
		}


	

		[HttpPost]
		[AuthorizeAjax(Roles = "Admin")]
		public async Task<ActionResult> GetUser(string username)
		{
			return Json(await AdminUserReader.GetUserDetails(username));
		}


		#region User

		[HttpGet]
		[AuthorizeAjax(Roles = "Admin")]
		public async Task<ActionResult> UpdateUser(string username)
		{
			var model = await AdminUserReader.GetUserUpdate(username).ConfigureAwait(false);
			if (model == null)
				return ViewMessageModal(new ViewMessageModel(ViewMessageType.Warning, "User Not Found!", $"Could not find user '{username}'"));

			return View("UpdateUserModal", model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[AuthorizeAjax(Roles = "Admin")]
		public async Task<ActionResult> UpdateUser(AdminUserUpdateModel model)
		{
			if (!ModelState.IsValid)
				return View("UpdateUserModal", model);

			var result = await AdminUserWriter.UpdateUser(model).ConfigureAwait(false);
			if (!ModelState.IsWriterResultValid(result))
				return View("UpdateUserModal", model);

			await ChatHub.InvalidateUserCache(model.UserId);
			return CloseModalSuccess(result.Message);
		}

		#endregion

		#region Settings

		[HttpGet]
		[AuthorizeAjax(Roles = "Admin")]
		public async Task<ActionResult> UpdateSettings(string username)
		{
			var model = await AdminUserReader.GetSettingsUpdate(username);
			if (model == null)
				return ViewMessageModal(new ViewMessageModel(ViewMessageType.Warning, "User Not Found!", $"Could not find user '{username}'"));

			return View("UpdateSettingsModal", model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[AuthorizeAjax(Roles = "Admin")]
		public async Task<ActionResult> UpdateSettings(AdminUserSettingsUpdateModel model)
		{
			if (!ModelState.IsValid)
				return View("UpdateSettingsModal", model);


			var result = await AdminUserWriter.UpdateSettings(User.Identity.GetUserId(), model);
			if (!ModelState.IsWriterResultValid(result))
				return View("UpdateSettingsModal", model);

			//ChatHub.InvalidateUserCache(model.UserId);
			return CloseModalSuccess(result.Message);
		}

		#endregion

		#region TwoFactor


		#endregion

		#region Api

		[HttpGet]
		[AuthorizeAjax(Roles = "Admin")]
		public async Task<ActionResult> UpdateApi(string username)
		{
			var model = await AdminUserReader.GetApiUpdate(username);
			if (model == null)
				return ViewMessageModal(new ViewMessageModel(ViewMessageType.Warning, "User Not Found!", $"Could not find user '{username}'"));

			return View("UpdateApiModal", model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[AuthorizeAjax(Roles = "Admin")]
		public async Task<ActionResult> UpdateApi(AdminUserApiUpdateModel model)
		{
			if (!ModelState.IsValid)
				return View("UpdateApiModal", model);

			var result = await AdminUserWriter.UpdateApi(User.Identity.GetUserId(), model);
			if (!ModelState.IsWriterResultValid(result))
				return View("UpdateApiModal", model);

			//ChatHub.InvalidateUserCache(model.UserId);
			return CloseModalSuccess(result.Message);
		}

		#endregion

		#region Profile

		[HttpGet]
		[AuthorizeAjax(Roles = "Admin")]
		public async Task<ActionResult> UpdateProfile(string username)
		{
			var model = await AdminUserReader.GetProfileUpdate(username).ConfigureAwait(false);
			if (model == null)
				return ViewMessageModal(new ViewMessageModel(ViewMessageType.Warning, "User Not Found!", $"Could not find user '{username}'"));

			return View("UpdateProfileModal", model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[AuthorizeAjax(Roles = "Admin")]
		public async Task<ActionResult> UpdateProfile(AdminUserProfileUpdateModel model)
		{
			if (!ModelState.IsValid)
				return View("UpdateProfileModal", model);

			var result = await AdminUserWriter.UpdateProfile(model);
			if (!ModelState.IsWriterResultValid(result))
				return View("UpdateProfileModal", model);

			//ChatHub.InvalidateUserCache(model.UserId);
			return CloseModalSuccess(result.Message);
		}

		#endregion
	}
}