using Cryptopia.Infrastructure.Common.DataTables;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using System.Web.Mvc;
using Cryptopia.Common.Chat;
using Web.Site.Extensions;
using Web.Site.Notifications;

namespace Web.Site.Controllers
{
	public class AdminChatController : BaseController
	{
		public IChatReader ChatReader { get; set; }

		[HttpGet]
		[AuthorizeAjax(Roles = "Admin, Moderator")]
		public async Task<ActionResult> GetChatAdmin()
		{
			var model = await ChatReader.GetChatUsers();
			return PartialView("_ChatAdmin", new ChatAdminModel
			{
				ChatUsers = model
			});
		}

		[HttpPost]
		[AuthorizeAjax(Roles = "Admin, Moderator")]
		public async Task<ActionResult> GetBannedUsers(DataTablesModel model)
		{
			return DataTable(await ChatReader.GetBannedUsers(model));
		}

		[HttpPost]
		[AuthorizeAjax(Roles = "Admin, Moderator")]
		public async Task<ActionResult> GetChatHistory(DataTablesModel model)
		{
			return DataTable(await ChatReader.GetChatMessages(model));
		}

		[HttpPost]
		[AuthorizeAjax(Roles = "Admin, Moderator")]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> SubmitChatbotMessage(string sender, string message)
		{
			await ChatHub.SendChatbotMessage(User.Identity.GetUserName(), sender, message);
			return JsonSuccess();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[AuthorizeAjax(Roles = "Admin, Moderator")]
		public async Task<ActionResult> SubmitChatBan(ChatBanUserModel model)
		{
			model.UserId = User.Identity.GetUserId();
			await ChatHub.BanUser(model);
			return JsonSuccess();
		}
	}
}