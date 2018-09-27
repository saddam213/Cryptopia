using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using Web.Site.Notifications;
using Microsoft.AspNet.Identity;
using Cryptopia.Data.DataContext;
using Cryptopia.Entity;
using Cryptopia.Enums;
using Cryptopia.Common.Notifications;

namespace Web.Site.Controllers
{
	public class KarmaController : BaseController
	{
		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> SubmitTipKarma(string chatHandle, int chatMessageId)
		{
			using (var context = new ApplicationDbContext())
			{
				var senderId = User.Identity.GetUserId();
				var sender = await context.Users.FirstOrDefaultAsync(x => x.Id == senderId).ConfigureAwait(false);
				if (sender == null)
				{
					return Json(new {Success = false, Message = Resources.Layout.karmaErrorUnauthorizedMessage });
				}

				var user = await context.Users.FirstOrDefaultAsync(x => x.ChatHandle == chatHandle || x.UserName == chatHandle).ConfigureAwait(false);
				if (user == null)
				{
					return Json(new {Success = false, Message = Resources.Layout.karmaErrorUserNotFoundMessage });
				}

				var chatBotId = Constant.SYSTEM_USER_CHATBOT.ToString();
				var tip = await context.ChatMessages.FirstOrDefaultAsync(x => x.Id == chatMessageId && x.UserId == chatBotId).ConfigureAwait(false);
				if (tip == null || tip.Message.Split(':')[1] != chatHandle)
				{
					return Json(new {Success = false, Message = Resources.Layout.karmaTipErrorNotFoundMessage });
				}

				var karma = new UserKarma
				{
					KarmaType = UserKarmaType.Tip,
					UserId = user.Id,
					SenderId = sender.Id,
					Discriminator = chatMessageId.ToString()
				};

				if (!await SaveKarmaChanges(context, user, karma))
				{
					return Json(new {Success = false, Message = string.Format(Resources.Layout.karmaTipErrorAlreadySentMessage, chatHandle)});
				}

				await SendKarmaNotification(context, user, string.Format(Resources.Layout.karmaTipNotificationMessage, sender.ChatHandle));
				await ChatHub.InvalidateUserCache(user.Id);
				return Json(new
				{
					Success = true,
					Message = string.Format(Resources.Layout.karmaTipIsSentMessage, chatHandle),
					User = user.UserName,
					Count = user.KarmaTotal
				});
			}
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> SubmitChatKarma(string chatHandle, int chatMessageId)
		{
			using (var context = new ApplicationDbContext())
			{
				var senderId = User.Identity.GetUserId();
				var sender = await context.Users.FirstOrDefaultAsync(x => x.Id == senderId).ConfigureAwait(false);
				if (sender == null)
				{
					return Json(new {Success = false, Message = Resources.Layout.karmaErrorUnauthorizedMessage});
				}

				var user = await context.Users.FirstOrDefaultAsync(x => x.ChatHandle == chatHandle || x.UserName == chatHandle).ConfigureAwait(false);
				if (user == null)
				{
					return Json(new {Success = false, Message = Resources.Layout.karmaErrorUserNotFoundMessage});
				}

				if (!await context.ChatMessages.AnyAsync(x => x.Id == chatMessageId && x.UserId == user.Id).ConfigureAwait(false))
				{
					return Json(new {Success = false, Message = Resources.Layout.karmaChatErrorMessageNotFoundMessage });
				}

				var maxTimestamp = DateTime.UtcNow.AddHours(-24);
				if (await context.UserKarma.CountAsync(x => x.SenderId == sender.Id && x.KarmaType == UserKarmaType.Chat && x.Timestamp > maxTimestamp).ConfigureAwait(false) >= 20)
				{
					return Json(new {Success = false, Message = Resources.Layout.karmaChatErrorLimitMessage });
				}

				var karma = new UserKarma
				{
					KarmaType = UserKarmaType.Chat,
					UserId = user.Id,
					SenderId = sender.Id,
					Discriminator = chatMessageId.ToString()
				};

				if (!await SaveKarmaChanges(context, user, karma))
				{
					return Json(new {Success = false, Message = string.Format(Resources.Layout.karmaChatErrorAlreadySentMessage, chatHandle)});
				}

				await SendKarmaNotification(context, user, string.Format(Resources.Layout.karmaChatNotificationMessage, sender.ChatHandle));
				await ChatHub.InvalidateUserCache(user.Id);
				return Json(new
				{
					Success = true,
					Message = string.Format(Resources.Layout.karmaChatIsSentMessage, chatHandle),
					User = user.UserName,
					Count = user.KarmaTotal
				});
			}
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> SubmitForumKarma(string username, int postId)
		{
			using (var context = new ApplicationDbContext())
			{
				var senderId = User.Identity.GetUserId();
				var sender = await context.Users.FirstOrDefaultAsync(x => x.Id == senderId);
				if (sender == null)
				{
					return Json(new {Success = false, Message = Resources.Layout.karmaErrorUnauthorizedMessage});
				}

				var user = await context.Users.FirstOrDefaultAsync(x => x.UserName == username);
				if (user == null)
				{
					return Json(new {Success = false, Message = Resources.Layout.karmaErrorUserNotFoundMessage});
				}

				if (!await context.ForumPosts.AnyAsync(x => x.Id == postId && x.UserId == user.Id))
				{
					return Json(new {Success = false, Message = Resources.Layout.karmaForumMessageNotFoundMessage });
				}

				var karma = new UserKarma
				{
					KarmaType = UserKarmaType.Forum,
					UserId = user.Id,
					SenderId = sender.Id,
					Discriminator = postId.ToString()
				};

				if (!await SaveKarmaChanges(context, user, karma))
				{
					return Json(new {Success = false, Message = string.Format(Resources.Layout.karmaForumErrorAlreadySentMessage, username)});
				}

				await SendKarmaNotification(context, user, string.Format(Resources.Layout.karmaForumNotificationMessage, sender.UserName));
				return Json(new
				{
					Success = true,
					Message = string.Format(Resources.Layout.karmaForumIsSentMessage, username),
					User = user.UserName,
					Count = user.KarmaTotal
				});
			}
		}

		private async Task<bool> SaveKarmaChanges(ApplicationDbContext context, ApplicationUser user, UserKarma karma)
		{
			try
			{
				context.UserKarma.Add(karma);
				user.KarmaTotal += 1;
				await context.SaveChangesAsync().ConfigureAwait(false);
				return true;
			}
			catch (DbUpdateException)
			{
				return false;
			}
		}

		private async Task SendKarmaNotification(ApplicationDbContext context, ApplicationUser user, string message)
		{
			if (user.DisableKarmaNotify)
			{
				return;
			}

			var notification = new Cryptopia.Entity.UserNotification
			{
				Type = "Info",
				UserId = user.Id,
				Title = "Karma Received",
				Notification = message,
				Acknowledged = false,
				Timestamp = DateTime.UtcNow
			};
			context.Notifications.Add(notification);
			await context.SaveChangesAsync().ConfigureAwait(false);
			var notificationHub = Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
			if (notificationHub != null)
			{
				var notificationModel = new NotificationModel
				{
					Header = "Karma Received",
					Notification = message,
					Type = NotificationLevelType.Info
				};
				await notificationHub.Clients.User(user.Id).SendNotification(notificationModel);
			}
		}
	}
}