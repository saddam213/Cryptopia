using Cryptopia.Cache;
using Cryptopia.Common.Chat;
using Cryptopia.Common.DataContext;
using Cryptopia.Data.DataContext;
using Cryptopia.Entity;
using Cryptopia.Enums;
using Cryptopia.Infrastructure.Common.DataContext;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using Web.Site.Helpers;

namespace Web.Site.Cache
{
	public class ChatCache
	{
		private readonly static int _historyCount = 100;
		private static bool _useRedisCache = bool.Parse(ConfigurationManager.AppSettings["Redis_ChatCache_Enabled"]);
		private static DistributedDictionary<ChatMessageData> _chatMessages = new DistributedDictionary<ChatMessageData>(RedisConnectionFactory.GetChatCacheConnection(), "ChatMessages", _useRedisCache);
		private static DistributedDictionary<ChatUser> _onlineUsers = new DistributedDictionary<ChatUser>(RedisConnectionFactory.GetChatCacheConnection(), "OnlineUsers", _useRedisCache);
		private static string _avatarPath = HostingEnvironment.MapPath("~/Content/Images/Avatar");

		public async Task<List<ChatMessageData>> GetMessages()
		{
			var chatData = await _chatMessages.GetValuesAsync();
			if (!chatData.Any())
			{
				await _chatMessages.AddRangeAsync(async () =>
				{
					using (var context = new ApplicationDbContext())
					{
						chatData = await context.ChatMessages
								.OrderByDescending(x => x.Timestamp)
								.Where(x => x.IsEnabled)
								.Take(_historyCount)
								.OrderBy(x => x.Timestamp)
								.Select(x => new ChatMessageData
								{
									Id = x.Id,
									IsEnabled = x.IsEnabled,
									Message = x.Message,
									Timestamp = x.Timestamp,
									UserId = x.UserId,
									ChatHandle = x.User.ChatHandle,
									Flair = x.User.RoleCss,
									KarmaTotal = x.User.KarmaTotal,
									UserName = x.User.UserName
								}).ToListNoLockAsync();

						foreach (var item in chatData)
						{
							if (item.UserId == Constant.SYSTEM_USER_CHATBOT.ToString())
							{
								var msgParts = item.Message.Split(':');
								if (msgParts.Length != 3)
								{
									item.IsEnabled = false;
									continue;
								}

								item.IsBot = msgParts[0] != "Tipbot";
								item.IsTipBot = msgParts[0] == "Tipbot";
								item.UserName = msgParts[0];
								item.ChatHandle = msgParts[1];
								item.Message = msgParts[2];
								continue;
							}

							item.Message = HttpUtility.HtmlEncode(item.Message);
							item.Avatar = GetAvatar(item.UserName);
						}
						return chatData.ToDictionary(k => k.Id.ToString(), v => v);
					}
				}, true);
			}
			return chatData.Where(x => x.IsEnabled)
				.OrderByDescending(x => x.Id)
				.Take(_historyCount)
				.OrderBy(x => x.Timestamp)
				.ToList();
		}

		private string GetAvatar(string userName)
		{
			string filename = string.Format("{0}\\{1}.png", _avatarPath, userName);
			string avatar = File.Exists(filename)
				? $"{CdnHelper.ImagePath()}/Content/Images/Avatar/{userName}.png"
				: $"{CdnHelper.ImagePath()}/Content/Images/Avatar.png";
			return avatar;
		}

		public async Task<ChatUser> GetChatUser(string userId)
		{
			try
			{
				var chatuser = await GetUserFromCache(userId);
				if (chatuser == null)
				{
					if (userId == Guid.Empty.ToString())
						return new ChatUser { Id = Guid.Empty.ToString(), Avatar = $"{CdnHelper.ImagePath()}/Content/Images/Avatar.png" };

					using (var context = new ApplicationDbContext())
					{
						var user = await context.Users
						.Where(x => x.Id == userId)
						.Select(x => new
						{
							Id = x.Id,
							UserName = x.UserName,
							ChatHandle = x.ChatHandle,
							ChatBanEndTime = x.ChatBanEndTime,
							RoleCss = x.RoleCss,
							KarmaTotal = x.KarmaTotal,
						}).FirstOrDefaultNoLockAsync();

						if (user == null)
							return new ChatUser { Id = Guid.Empty.ToString(), Avatar = $"{CdnHelper.ImagePath()}/Content/Images/Avatar.png" };

						chatuser = new ChatUser
						{
							Id = user.Id,
							UserName = HttpUtility.HtmlEncode(user.UserName),
							ChatHandle = HttpUtility.HtmlEncode(user.ChatHandle),
							BanEndTime = user.ChatBanEndTime,
							RoleCss = user.RoleCss,
							Avatar = GetAvatar(user.UserName),
							KarmaTotal = user.KarmaTotal
						};
						await AddUserToCache(userId, chatuser);
					}

				}
				return chatuser;
			}
			catch (Exception)
			{
				return null;
			}
		}

		public async Task AddMessage(ChatMessageData message)
		{
			try
			{
				var messageCache = await GetMessages();
				if (messageCache.Count > _historyCount)
				{
					await _chatMessages.RemoveAsync(messageCache.Min(x => x.Id).ToString());
				}

				using (var context = new ApplicationDbContext())
				{
					var chatMessage = new ChatMessage
					{
						Message = message.Message,
						Timestamp = message.Timestamp,
						UserId = message.UserId,
						IsEnabled = true
					};
					context.ChatMessages.Add(chatMessage);
					await context.SaveChangesAsync();

					message.Id = chatMessage.Id;
					await _chatMessages.AddAsync(message.Id.ToString(), message);
				}
			}
			catch (Exception)
			{
			}
		}

		public async Task RemoveMessage(int messageId)
		{
			using (var context = new ApplicationDbContext())
			{
				var chatItem = await context.ChatMessages.FirstOrDefaultNoLockAsync(x => x.Id == messageId);
				if (chatItem != null)
				{
					chatItem.IsEnabled = false;
					await context.SaveChangesAsync();
				}
			}
			await _chatMessages.RemoveAsync(messageId.ToString());
		}

		public async Task<List<ChatUser>> GetOnlineUsers()
		{
			return await _onlineUsers.GetValuesAsync();
		}

		public async Task AddOnlineUser(string userId)
		{
			if (!await _onlineUsers.ExistsAsync(userId))
				await _onlineUsers.AddAsync(userId, await GetChatUser(userId));
		}

		public async Task RemoveOnlineUser(string userId)
		{
			await _onlineUsers.RemoveAsync(userId);
		}

		public async Task<int> GetOnlineUsersCount()
		{
			return await _onlineUsers.GetCountAsync();
		}

		private async Task<ChatUser> GetUserFromCache(string userId)
		{
			return await _onlineUsers.GetValueAsync(userId);
		}

		private async Task AddUserToCache(string userId, ChatUser chatuser)
		{
			await _onlineUsers.AddAsync(userId, chatuser);
		}

		private bool IsAdmin(List<string> roles)
		{
			if (roles == null || !roles.Any())
				return false;

			return roles.Any(x =>
					 x.Equals("89CB4B27-59A7-4F90-8D84-A6498197D74B", StringComparison.OrdinalIgnoreCase)
				|| x.Equals("76150DF9-E821-4500-8D02-81A0042BA53A", StringComparison.OrdinalIgnoreCase)
				|| x.Equals("0871A531-B929-4912-972C-51A05DFE6A11", StringComparison.OrdinalIgnoreCase)
			);
		}
	}
}
