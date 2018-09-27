using Cryptopia.Common.Chat;
using Cryptopia.Common.DataContext;
using Cryptopia.Data.DataContext;
using Cryptopia.Enums;
using Cryptopia.Infrastructure.Common.DataContext;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Web.Site.Cache;
using Web.Site.Helpers;

namespace Web.Site.Notifications
{
	public class ChatHub : Hub
	{
		private static ChatCache _cache = new ChatCache();
		public static ChatCache Cache
		{
			get { return _cache; }
		}

		private static ConcurrentDictionary<string, byte> _serverUsers = new ConcurrentDictionary<string, byte>();

		public override async Task OnConnected()
		{
			var userId = CurrentUserId();
			if (!string.IsNullOrEmpty(userId) && !_serverUsers.ContainsKey(userId))
			{
				if (_serverUsers.TryAdd(userId, 0))
				{
					await _cache.AddOnlineUser(userId);
				}
			}
			await base.OnConnected();
		}

		public override async Task OnDisconnected(bool stopCalled)
		{
			var userId = CurrentUserId();
			if (!string.IsNullOrEmpty(userId) && _serverUsers.ContainsKey(userId))
			{
				byte _out;
				if (_serverUsers.TryRemove(userId, out _out))
				{
					await _cache.RemoveOnlineUser(userId);
				}
			}
			await base.OnDisconnected(stopCalled);
		}

		#region Static Methods

		public static async Task<List<ChatMessageData>> GetUserChatCache()
		{
			return await _cache.GetMessages();
		}

		public static async Task InvalidateUserCache(string userId)
		{
			if (!string.IsNullOrEmpty(userId))
			{
				byte _out;
				_serverUsers.TryRemove(userId, out _out);
				await _cache.RemoveOnlineUser(userId);
			}
		}

		[Authorize]
		public static async Task SendChatbotMessage(string name, string sender, string message)
		{
			try
			{
				var hub = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
				if (hub != null)
				{
					var botMessage = ChatHelper.FilterTextBot(message);
					var chatItem = new ChatMessageData
					{
						UserId = Constant.SYSTEM_USER_CHATBOT.ToString(),
						Message = HttpUtility.HtmlEncode($"{name}:{sender}:{botMessage}"),
						IsEnabled = true,
						Timestamp = DateTime.UtcNow,
						UserName = name,
						ChatHandle = sender,
						IsBot = true,
					};
					await _cache.AddMessage(chatItem);

					chatItem.Message = botMessage;
					await BroadcastMessage(hub, chatItem);
				}
			}
			catch (Exception)
			{
			}
		}

		[Authorize]
		public static async Task SendTipbotMessage(string sender, string message)
		{
			try
			{
				var hub = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
				if (hub != null)
				{
					var botMessage = ChatHelper.FilterTextBot(message);
					var chatItem = new ChatMessageData
					{
						UserId = Constant.SYSTEM_USER_CHATBOT.ToString(),
						Message = HttpUtility.HtmlEncode($"Tipbot:{sender}:{botMessage}"),
						IsEnabled = true,
						Timestamp = DateTime.UtcNow,
						UserName = "Tipbot",
						ChatHandle = sender,
						IsTipBot = true
					};
					await _cache.AddMessage(chatItem);

					chatItem.Message = botMessage;
					await BroadcastMessage(hub, chatItem);
				}
			}
			catch (Exception)
			{
			}
		}



		public static async Task BanUser(ChatBanUserModel model)
		{
			try
			{
				using (var context = new ApplicationDbContext())
				{
					var banner = await context.Users.FirstOrDefaultNoLockAsync(x => x.Id == model.UserId);
					var user = await context.Users.FirstOrDefaultNoLockAsync(x => x.Id == model.BanUserId);
					if (user != null && banner != null)
					{
						if (model.Seconds == 0 && model.BanType != ChatBanType.Warning)
						{
							user.ChatBanEndTime = DateTime.UtcNow;
							user.ChatTipBanEndTime = DateTime.UtcNow;
							await SendChatbotMessage(banner.UserName, "Ban Hammer", $"{user.ChatHandle} has been unbanned. {model.Message}");
						}
						else
						{
							string message = string.Empty;
							switch (model.BanType)
							{
								case ChatBanType.Warning:
									await SendChatbotMessage(banner.UserName, "Ban Hammer", $"{user.ChatHandle} {model.Message}");
									return;
								case ChatBanType.ChatOnly:
									user.ChatBanEndTime = DateTime.UtcNow.AddSeconds(model.Seconds);
									message = $"{banner.ChatHandle} has banned {user.ChatHandle} from chat for {model.Seconds} seconds.";
									break;
								case ChatBanType.TipOnly:
									user.ChatTipBanEndTime = DateTime.UtcNow.AddSeconds(model.Seconds);
									message = $"{banner.ChatHandle} has banned {user.ChatHandle} from tips for {model.Seconds} seconds.";
									break;
								case ChatBanType.All:
									user.ChatBanEndTime = DateTime.UtcNow.AddSeconds(model.Seconds);
									user.ChatTipBanEndTime = DateTime.UtcNow.AddSeconds(model.Seconds);
									message = $"{banner.ChatHandle} has banned {user.ChatHandle} from chat & tips for {model.Seconds} seconds.";
									break;
								default:
									break;
							}

							if (!string.IsNullOrEmpty(message))
							{
								if (!string.IsNullOrEmpty(model.Message))
								{
									message = $"{message} Reason: {model.Message}";
								}
								await SendChatbotMessage(banner.UserName, "Ban Hammer", message);
							}
						}

						await _cache.RemoveOnlineUser(user.Id);
						await context.SaveChangesAsync();
					}
				}
			}
			catch (Exception)
			{
			}
		}

		private static async Task BroadcastMessage(IHubContext hub, ChatMessageData message)
		{
			try
			{
				await hub.Clients.All.broadcastMessage(new
				{
					Id = message.Id,
					UserName = message.UserName,
					ChatHandle = message.ChatHandle,
					Flair = message.Flair,
					Timestamp = message.Timestamp.ToString("MM/dd/yyyy HH:mm:ss"),
					Avatar = message.Avatar,
					Message = message.Message,
					KarmaTotal = message.KarmaTotal,
					IsBot = message.IsBot,
					IsTipBot = message.IsTipBot
				});
			}
			catch (Exception)
			{
			}
		}

		#endregion

		private string CurrentUserId()
		{
			if (Context.User != null && Context.User.Identity != null)
			{
				return Context.User.Identity.GetUserId();
			}
			return null;
		}
	}
}