using Cryptopia.Common.Chat;
using Cryptopia.Common.Notifications;
using Cryptopia.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Web.Site.Helpers
{
	public static class ChatHelper
	{
		public static string FilterText(string text)
		{
			if (text.Count(x => x == '[') > 10)
			{
				return text.Replace("[", "").Replace("]", "").Truncate(500);
			}
			return text.Truncate(500);
		}

		public static string FilterTextBot(string text)
		{
			if (text.Count(x => x == '[') > 1)
			{
				return text.Replace("[", "").Replace("]", "").Truncate(200);
			}
			return text.Truncate(200);
		}

		public static NotificationModel ValidateMessage(IEnumerable<ChatMessageData> chatData, ChatUser sender, string message)
		{
			if (sender.BanEndTime.HasValue && sender.BanEndTime.Value > DateTime.UtcNow)
			{
				return new NotificationModel
				{
					Header = "Chat Admin",
					Notification = $"You are currently banned from chat for another {(sender.BanEndTime.Value - DateTime.UtcNow).TotalSeconds} seconds",
					Type = NotificationLevelType.Error
				};
			}
			else
			{
				var trimmed = message.Trim();
				var last = chatData.LastOrDefault();
				if ((last != null && last.Message == message) || _bannedMessages.Contains(trimmed.ToLower()))
				{
					return new NotificationModel
					{
						Header = "Spam Blocker",
						Notification = "Spam filter rejected duplicate message",
						Type = NotificationLevelType.Error
					};
				}

				var spammer = chatData.Skip(chatData.Count() - 3).Take(3).Count(x => x.IsEnabled && x.UserId == sender.Id) == 3;
				if (spammer)
				{
					return new NotificationModel
					{
						Header = "Spam Blocker",
						Notification = "Spam filter rejected 3rd consecutive message",
						Type = NotificationLevelType.Error
					};
				}

				// Check if this is a gif only
				if (trimmed.StartsWith("[") && trimmed.EndsWith("]"))
				{
					var gifspammer = chatData.Where(x => x.UserId == sender.Id)
							.OrderByDescending(x => x.Timestamp)
							.Take(3)
							.Count(x => x.IsEnabled && x.Message.Trim().StartsWith("[") && x.Message.Trim().EndsWith("]")) == 3;
					if (gifspammer)
					{
						return new NotificationModel
						{
							Header = "Spam Blocker",
							Notification = "Spam filter rejected 3rd consecutive emoticon",
							Type = NotificationLevelType.Error
						};
					}
				}
				return null;
			}
		}

		private static List<string> _bannedMessages = new List<string>
		{
			"ty",
			"ty!",
			"thanks",
			"thanks!",
			"tnx",
			"tnx!",
			"tvm",
			"tvm",
			"."
		};
	}
}
