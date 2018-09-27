using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cryptopia.Common.User;
using Cryptopia.Infrastructure.Common.DataContext;
using Cryptopia.Common.Notifications;
using Cryptopia.Enums;
using Cryptopia.Infrastructure.Common.Results;
using Ganss.XSS;

namespace Cryptopia.Core.User
{
	public class UserMessageWriter : IUserMessageWriter
	{
		public IDataContextFactory DataContextFactory { get; set; }
		public INotificationService NotificationService { get; set; }
		public async Task<IWriterResult> CreateMessage(string userId, UserMessageCreateModel model)
		{
			try
			{
				using (var context = DataContextFactory.CreateContext())
				{
					var sender = await context.Users.Where(x => x.Id == userId).Select(x => x.UserName).FirstOrDefaultNoLockAsync().ConfigureAwait(false);
					var usernames = model.Recipiants.Split(';').Select(x => x.Trim()).ToList();
					var recipients = await context.Users
						.Where(x => usernames.Contains(x.UserName))
						.Select(x => x.Id)
						.ToListNoLockAsync().ConfigureAwait(false);
					if (!recipients.Any())
						return new WriterResult(false, "No users found.");

					var sanitizedMessage = SanitizeMessage(model.Message);
					var newMessages = new List<Entity.UserMessage>();
					foreach (var recipient in recipients)
					{
						var inbox = new Entity.UserMessage
						{
							IsInbound = true,
							SenderUserId = userId,
							UserId = recipient,
							IsRead = false,
							Timestamp = DateTime.UtcNow,
							Subject = model.Subject,
							Message = sanitizedMessage
						};
						newMessages.Add(inbox);
					}

					var outbox = new Entity.UserMessage
					{
						IsInbound = false,
						SenderUserId = userId,
						UserId = userId,
						IsRead = true,
						Recipients = model.Recipiants,
						Timestamp = DateTime.UtcNow,
						Subject = model.Subject,
						Message = sanitizedMessage
					};
					context.Messages.Add(outbox);
					foreach (var message in newMessages)
					{
						context.Messages.Add(message);
					}
					await context.SaveChangesAsync().ConfigureAwait(false);
					await SendOutboxNotification(outbox).ConfigureAwait(false);
					await SendInboxNotifications(sender, newMessages).ConfigureAwait(false);

					return new WriterResult(true, "Message sent successfully.");
				}
			}
			catch (Exception)
			{
				return new WriterResult(false);
			}
		}

		public async Task<IWriterResult> DeleteMessage(string userId, int messageId)
		{
			try
			{
				using (var context = DataContextFactory.CreateContext())
				{
					var message =	await context.Messages.Where(m => m.Id == messageId && m.UserId == userId).FirstOrDefaultNoLockAsync().ConfigureAwait(false);
					if (message == null)
						return new WriterResult(false, "Message not found.");

					context.Messages.Remove(message);
					await context.SaveChangesAsync().ConfigureAwait(false);
					return new WriterResult(true, "Message successfully deleted.");
				}
			}
			catch (Exception)
			{
				return new WriterResult(false);
			}
		}

		public async Task<IWriterResult> DeleteAllMessage(string userId, bool isInbox)
		{
			try
			{
				using (var context = DataContextFactory.CreateContext())
				{
					var messages = await context.Messages.Where(m => m.IsInbound == isInbox && m.UserId == userId).ToListNoLockAsync().ConfigureAwait(false);
					if (!messages.Any())
						return new WriterResult(false, "Messages not found.");

					context.Messages.RemoveRange(messages);
					await context.SaveChangesAsync().ConfigureAwait(false);
					return new WriterResult(true, "Messages successfully deleted.");
				}
			}
			catch (Exception)
			{
				return new WriterResult(false);
			}
		}

		public async Task<IWriterResult> ReportMessage(string userId, UserMessageReportModel model)
		{
			try
			{
				using (var context = DataContextFactory.CreateContext())
				{
					await Task.Delay(1).ConfigureAwait(false);
					return new WriterResult(true,
						"This message has been reported to the site administrators, they will review and take the appropriate action.");
				}
			}
			catch (Exception)
			{
				return new WriterResult(false);
			}
		}

		private async Task SendInboxNotifications(string sender, List<Entity.UserMessage> messages)
		{
			try
			{
				foreach (var message in messages)
				{
					var notification = new UserMessageModel
					{
						Id = message.Id,
						Sender = sender,
						Subject = message.Subject,
						Timestamp = message.Timestamp
					};
					await SendNotification(DataNotificationType.InboxMessage, message.UserId, notification).ConfigureAwait(false);
					await NotificationService.CreateNotification(new CreateNotificationModel
					{
						UserId = message.UserId,
						Title = "New Message",
						Message = string.Format("New private message from {0}", sender),
						Type = NotificationType.Message,
						Level = NotificationLevelType.Info,
					}).ConfigureAwait(false);
				}
			}
			catch (Exception)
			{
			}
		}

		private async Task SendOutboxNotification(Entity.UserMessage message)
		{
			try
			{
				var notification = new UserMessageModel
				{
					Id = message.Id,
					Recipiants = message.Recipients,
					Subject = message.Subject,
					Timestamp = message.Timestamp
				};
				await SendNotification(DataNotificationType.OutboxMessage, message.UserId, notification).ConfigureAwait(false);
			}
			catch (Exception)
			{
			}
		}

		private async Task SendNotification(DataNotificationType type, string userId, UserMessageModel message = null)
		{
			await NotificationService.SendDataNotification(new DataNotificationModel
			{
				Data = message,
				Type = type,
				UserId = new Guid(userId)
			}).ConfigureAwait(false);
		}

		private string SanitizeMessage(string message)
		{
			var sanitizer = new HtmlSanitizer();
			return sanitizer.Sanitize(message);
		}
	}
}