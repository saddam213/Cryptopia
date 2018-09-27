using System;
using Cryptopia.Infrastructure.Common.DataTables;
using System.Threading.Tasks;
using System.Web.Mvc;
using Cryptopia.Common.Security;
using Cryptopia.Enums;
using Web.Site.Extensions;

namespace Web.Site.Controllers
{
	public class AdminSecurityController : BaseUserController
	{
		public ISecurityReader SecurityReader { get; set; }

		[HttpGet]
		[AuthorizeAjax(Roles = "Admin, Moderator")]
		public ActionResult GetSecurity()
		{
			return PartialView("_Security");
		}

		[HttpPost]
		[AuthorizeAjax(Roles = "Admin, Moderator")]
		public async Task<ActionResult> GetLockedOutUsers(DataTablesModel model)
		{
			return DataTable(await SecurityReader.GetLockedOutUsers(model));
		}

		[HttpPost]
		[AuthorizeAjax(Roles = "Admin, Moderator")]
		public async Task<ActionResult> GetUserLogons(DataTablesModel model)
		{
			return DataTable(await SecurityReader.GetUserLogons(model));
		}

		[HttpPost]
		[AuthorizeAjax(Roles = "Admin, Moderator")]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> UnlockUser(string username, bool sendemail)
		{
			var user = await UserManager.FindByNameAsync(username);
			if (user == null)
				return JsonError("User not found??");

			user.LockoutEndDateUtc = null;
			await UserManager.UpdateAsync(user);
			if (sendemail)
			{
				if (await SendEmailAsync(EmailTemplateType.SupportUnlockUser, null, user.Email, user.Id, user.UserName))
				{
					return JsonSuccess($"User '{username}' Successfully Unlocked, User has been notified");
				}
			}
			return JsonSuccess($"User '{username}' Successfully Unlocked, User has NOT been notified.");
		}

		[HttpPost]
		[AuthorizeAjax(Roles = "Admin, Moderator")]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> LockUser(string username)
		{
			var user = await UserManager.FindByNameAsync(username);
			if (user == null)
				return JsonError("User not found??");

			user.LockoutEndDateUtc = DateTime.UtcNow.AddYears(1);
			await UserManager.UpdateAsync(user);
			return JsonSuccess($"User '{username}' Successfully Locked.");
		}
	}
}