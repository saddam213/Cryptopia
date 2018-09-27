using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Web.Site.Models.Chat;
using System.IO;
using System.Web.Script.Serialization;
using Cryptopia.Common.Chat;
using Microsoft.AspNet.Identity;
using Web.Site.Notifications;
using Cryptopia.Common.Emoticons;
using System;
using System.Web;
using Web.Site.Helpers;

namespace Web.Site.Controllers
{
	public class ChatController : BaseController
	{
		public IChatReader ChatReader { get; set; }
		public IEmoticonReader EmoticonReader { get; set; }

		[HttpGet]
		[AllowAnonymous]
		public ActionResult Index()
		{
			return View("Chat");
		}

		[ChildActionOnly]
		[Authorize(Roles = "Admin")]
		public ActionResult ChatSettings()
		{
			return PartialView("_ChatSettingsPartial", new ChatSettingsModel());
		}

		[HttpGet]
		public async Task<ActionResult> Emoticons()
		{
			var emoticons = await EmoticonReader.GetEmoticons(Server.MapPath("~/Content/Images/EmoticonSet/Emoticon.json"));
			var categories = emoticons.Select(x => x.Category).Where(x => x != "All").Distinct().OrderBy(x => x);
			return View("ChatEmoticonModal", new ChatEmoticonViewModel
			{
				Emoticons = emoticons,
				Categories = new List<string>(categories)
			});
		}

		[HttpGet]
		[OutputCache(Duration = 300)]
		public async Task<ActionResult> EmoticonSet()
		{
			return Json(await EmoticonReader.GetEmoticons(Server.MapPath("~/Content/Images/EmoticonSet/Emoticon.json")), JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		[Authorize(Roles = "Admin, Moderator, ChatModerator")]
		public async Task<ActionResult> BanChatUser(string chathandle)
		{
			var selectedUserId = string.Empty;
			var users = await ChatReader.GetChatUsers();
			var selectedUser = users.FirstOrDefault(x => x.ChatHandle == chathandle);
			if (selectedUser != null)
				selectedUserId = selectedUser.UserId;

			return View("BanHammerModal", new ChatBanModel
			{
				UserId = selectedUserId,
				ChatUsers = users
			});
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = "Admin, Moderator, ChatModerator")]
		public async Task<ActionResult> BanChatUser(ChatBanUserModel model)
		{
			model.UserId = User.Identity.GetUserId();
			await ChatHub.BanUser(model);
			return CloseModalSuccess();
		}

		[HttpGet]
		public ActionResult ChatRules()
		{
			return View("RulesModal");
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> SendChatMessage(string message)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(message))
					return JsonError(Resources.Chat.errorEmptyMessage);

				var userId = User.Identity.GetUserId();
				if (string.IsNullOrEmpty(userId))
					return JsonError(Resources.Chat.errorUserNotFoundMessage);

				var sender = await ChatHub.Cache.GetChatUser(userId);
				if (sender == null)
					return JsonError(Resources.Chat.errorUserNotFoundMessage);

				var chatData = await ChatHub.Cache.GetMessages();
				var result = ChatHelper.ValidateMessage(chatData.OrderBy(x => x.Id), sender, message);
				if (result != null)
				{
					var hub = Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
					if (hub != null)
						await hub.Clients.User(sender.Id).SendNotification(result);

					return JsonError(result.Notification);
				}

				var chatItem = new ChatMessageData
				{
					UserId = sender.Id.ToString(),
					Message = HttpUtility.HtmlEncode(ChatHelper.FilterText(message)),
					IsEnabled = true,
					Timestamp = DateTime.UtcNow,
					UserName = sender.UserName,
					ChatHandle = sender.ChatHandle,
					Flair = sender.RoleCss,
					Avatar = sender.Avatar,
					KarmaTotal = sender.KarmaTotal
				};

				await ChatHub.Cache.AddMessage(chatItem);
				var chathub = Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
				if (chathub != null)
				{
					await chathub.Clients.All.broadcastMessage(new
					{
						Id = chatItem.Id,
						UserName = chatItem.UserName,
						ChatHandle = chatItem.ChatHandle,
						Flair = chatItem.Flair,
						Timestamp = chatItem.Timestamp.ToString("MM/dd/yyyy HH:mm:ss"),
						Avatar = chatItem.Avatar,
						Message = chatItem.Message,
						KarmaTotal = chatItem.KarmaTotal
					});
				}
				return JsonSuccess();
			}
			catch (Exception)
			{
				return JsonError(Resources.Chat.errorUnknownMessage);
			}
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = "Admin, Moderator, ChatModerator")]
		public async Task<ActionResult> RemoveChatPost(int chatId)
		{
			await ChatHub.Cache.RemoveMessage(chatId);
			var chathub = Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
			if (chathub != null)
			{
				await chathub.Clients.All.broadcastRemoval(chatId);
			}
			return JsonSuccess();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[OutputCache(Duration = 10, VaryByParam ="none", VaryByCustom ="User")]
		public async Task<ActionResult> GetOnlineCount()
		{
			var caller = User.Identity.IsAuthenticated ? User.Identity.GetUserId() : Guid.Empty.ToString();
			var onlineUsers = await ChatHub.Cache.GetOnlineUsersCount();
			var chathub = Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
			if (chathub != null)
			{
				await chathub.Clients.User(caller).broadcastOnlineCount(onlineUsers, DateTime.UtcNow.ToString("MM/dd/yyyy HH:mm:ss"));
			}
			return JsonSuccess();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[OutputCache(Duration = 1, VaryByParam = "none")]
		public async Task<ActionResult> GetChatHistory()
		{
			var history = await ChatHub.GetUserChatCache();
			return Json(new
			{
				data = history.Select(chatdata => new
				{
					Id = chatdata.Id,
					UserName = chatdata.UserName,
					ChatHandle = chatdata.ChatHandle,
					Flair = chatdata.Flair,
					Timestamp = chatdata.Timestamp.ToString("MM/dd/yyyy HH:mm:ss"),
					Avatar = chatdata.Avatar,
					Message = chatdata.Message,
					KarmaTotal = chatdata.KarmaTotal,
					IsBot = chatdata.IsBot,
					IsTipBot = chatdata.IsTipBot
				}).ToList(),
				time = DateTime.UtcNow.ToString("MM/dd/yyyy HH:mm:ss")
			});
		}
	}
}