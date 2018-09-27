using Cryptopia.Admin.Common.Chat;
using Cryptopia.Infrastructure.Common.DataTables;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using System.Web.Mvc;
using Web.Admin.Helpers;
using Web.Admin.Models;
using Cryptopia.Admin.Common.News;
using Cryptopia.Common.News;
using Cryptopia.Admin.Common.Security;
using Cryptopia.Enums;
using Cryptopia.Admin.Common.Referral;
using Cryptopia.Admin.Common.AdminUser;

namespace Web.Admin.Controllers
{
	[Authorize(Roles = "Admin")]
	public class SiteManagementController : BaseUserController
	{
		public IAdminChatReader AdminChatReader { get; set; }
		public IAdminNewsReader NewsReader { get; set; }
		public IAdminNewsWriter NewsWriter { get; set; }
		public IAdminSecurityReader SecurityReader { get; set; }
		public IAdminReferralReader ReferralReader { get; set; }
        public IAdminUserWriter UserWriter { get; set; }

		public ActionResult Index()
		{
			return View();
		}

		public async Task<ActionResult> GetBannedUsers(DataTablesModel model)
		{
			return DataTable(await AdminChatReader.GetBannedUsers(model));
		}

		#region News

		[HttpPost]
		public async Task<ActionResult> GetNews(DataTablesModel model)
		{
			return DataTable(await NewsReader.GetNews(model));
		}

		[HttpGet]
		public ActionResult CreateNews()
		{
			return View("CreateNewsModal", new CreateNewsItemModel());
		}

		[HttpPost]
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
		public async Task<ActionResult> UpdateNews(int id)
		{
			var model = await NewsReader.GetNewsItem(id);
			if (model == null)
				return ViewMessageModal(new ViewMessageModel(ViewMessageType.Danger, "Error", $"News item '{id}' not found"));

			return View("UpdateNewsModal", model);
		}

		[HttpPost]
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

		#endregion

		#region AdminSecurity

		[HttpPost]
		public async Task<ActionResult> GetLockedOutUsers(DataTablesModel model)
		{
			return DataTable(await SecurityReader.GetLockedOutUsers(model));
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> UnlockUser(string username, bool sendemail)
		{
			var user = await UserManager.FindByNameAsync(username);
			if (user == null)
				return JsonError("User not found??");

			user.LockoutEndDateUtc = null;
            await UserWriter.UnlockUser(User.Identity.GetUserId(), username);
			//await UserManager.UpdateAsync(user);

			if (sendemail)
			{
				if (await SendEmailAsync(EmailTemplateType.SupportUnlockUser, null, user.Email, user.Id, user.UserName))
				{
					return JsonSuccess($"User '{username}' Successfully Unlocked, User has been notified");
				}
			}
			return JsonSuccess($"User '{username}' Successfully Unlocked, User has NOT been notified.");
		}

		#endregion

		#region Referrals
		[HttpPost]
		public async Task<ActionResult> GetReferrals(DataTablesModel model)
		{
			return DataTable(await ReferralReader.GetHistory(model));
		}
		#endregion
	}
}