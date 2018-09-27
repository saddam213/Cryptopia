using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cryptopia.Common.User;
using Cryptopia.Infrastructure.Common.DataContext;
using Cryptopia.Common.Notifications;
using Cryptopia.Enums;
using Ganss.XSS;


namespace Cryptopia.Core.User
{
	public class UserMessageReader : IUserMessageReader
	{
		private readonly string SQL_UPDATE_SETMESSAGEREAD = "UPDATE UserMessage SET IsRead = 1 WHERE Id = @p0";
		public IDataContextFactory DataContextFactory { get; set; }

		public async Task<UserMessageModel> GetMessage(string userId, int messageId)
		{
			try
			{
				using (var context = DataContextFactory.CreateReadOnlyContext())
				{
					var message = await context.Messages
						.AsNoTracking()
						.Where(m => m.Id == messageId && (m.SenderUserId == userId || m.UserId == userId))
						.Select(m => new UserMessageModel
						{
							Id = m.Id,
							IsInbound = m.IsInbound,
							IsRead = m.IsRead,
							Message = m.Message,
							Sender = m.Sender.UserName,
							Subject = m.Subject,
							Timestamp = m.Timestamp,
							Recipiants = m.Recipients
						}).OrderByDescending(x => x.Id)
						.FirstOrDefaultNoLockAsync().ConfigureAwait(false);

					if (!message.IsRead)
					{
						await context.Database.ExecuteSqlCommandAsync(SQL_UPDATE_SETMESSAGEREAD, message.Id).ConfigureAwait(false);
					}

					message.Message = SanitizeMessage(message.Message);
					return message;
				}
			}
			catch (Exception)
			{
				return new UserMessageModel();
			}
		}

		public async Task<List<UserMessageModel>> GetMessages(string userId)
		{
			try
			{
				using (var context = DataContextFactory.CreateReadOnlyContext())
				{
					var messages = await context.Messages
						.AsNoTracking()
						.Where(m => (!m.IsInbound && m.SenderUserId == userId) || (m.IsInbound && m.UserId == userId))
						.Select(m => new UserMessageModel
						{
							Id = m.Id,
							IsInbound = m.IsInbound,
							IsRead = m.IsRead,
							Sender = m.Sender.UserName,
							Subject = m.Subject,
							Timestamp = m.Timestamp,
							Recipiants = m.Recipients
						}).OrderByDescending(x => x.Id)
						.ToListNoLockAsync().ConfigureAwait(false);

					return messages;
				}
			}
			catch (Exception)
			{
				return new List<UserMessageModel>();
			}
		}

		private string SanitizeMessage(string message)
		{
			var sanitizer = new HtmlSanitizer();
			return sanitizer.Sanitize(message);
		}
	}
}