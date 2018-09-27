namespace Cryptopia.Admin.Core.Support
{
	using System;
	using System.Data.Entity;
	using System.Linq;
	using System.Threading.Tasks;
	using Cryptopia.Admin.Common.Support;
	using Cryptopia.Entity.Support;
	using Cryptopia.Enums;
	using Cryptopia.Infrastructure.Common.DataContext;
	using Cryptopia.Infrastructure.Common.Results;

	public class TicketMessageWriter : ITicketMessageWriter
	{
		public IDataContextFactory DataContextFactory { get; set; }

		public async Task<IWriterResult> CreateMessage(int ticketId, string adminUserId, string message)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var ticketMessage = SupportTicketMessage.CreateMessage(adminUserId, ticketId, message);
				context.SupportTicketMessage.Add(ticketMessage);
				await context.SaveChangesAsync().ConfigureAwait(false);

				return new WriterResult(true, $"New Message added to Ticket - {ticketId}");
			}
		}

		public async Task<IWriterResult> CreateAdminMessage(int ticketId, string adminUserId, string message)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var ticketMessage = SupportTicketMessage.CreateAdminMessage(adminUserId, ticketId, message);

				context.SupportTicketMessage.Add(ticketMessage);
				await context.SaveChangesAsync().ConfigureAwait(false); ;

				return new WriterResult(true, $"New Message added to Ticket - {ticketId}");
			}
		}

		public async Task<IWriterResult> DeleteMessage(int messageId, string adminUserId)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var ticketMessage = await context.SupportTicketMessage
					.Where(m => m.Id == messageId)
					.FirstAsync();

				if (!ticketMessage.IsDraft || ticketMessage.SenderId != adminUserId)
				{
					return new WriterResult(false, "Message could not be deleted");
				}

				context.SupportTicketMessage.Remove(ticketMessage);
				await context.SaveChangesAsync().ConfigureAwait(false); ;

				return new WriterResult(true, $"Message ${messageId} removed");
			}
		}

		public async Task<IWriterResult<PublishReplyResultModel>> PublishMessage(int messageId, string adminUserId)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var ticketProjection = await context.SupportTicketMessage
					.Include(m => m.Ticket)
					.Where(m => m.Id == messageId)
					.Select(m => new {
						TicketMessage = m,
						m.TicketId,
						m.Sender.Email,
						m.Sender.UserName
					})
					.FirstNoLockAsync().ConfigureAwait(false);

				var ticketMessage = ticketProjection.TicketMessage;

				if (ticketMessage.SenderId != adminUserId || !ticketMessage.IsDraft)
					return new WriterResult<PublishReplyResultModel>(false, default(PublishReplyResultModel));

				ticketMessage.IsDraft = false;
				ticketMessage.LastUpdate = DateTime.UtcNow;
				ticketMessage.Ticket.Status = SupportTicketStatus.AdminReply;
				ticketMessage.Ticket.LastUpdate = DateTime.UtcNow;
				await context.SaveChangesAsync().ConfigureAwait(false);

				return new WriterResult<PublishReplyResultModel>(true, new PublishReplyResultModel {
					UserId = ticketMessage.SenderId,
					TicketId = ticketMessage.TicketId,
					Email = ticketProjection.Email,
					UserName = ticketProjection.UserName
				});
			}
		}

		public async Task<IWriterResult> EditMessage(int messageId, string adminUserId, string message)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var ticketMessage = await context.SupportTicketMessage
					.Where(m => m.Id == messageId)
					.FirstNoLockAsync().ConfigureAwait(false);

				if (ticketMessage.SenderId != adminUserId || (!ticketMessage.IsInternal && !ticketMessage.IsDraft))
					return new WriterResult(false, "Message could not be Edited");

				ticketMessage.Message = message;
				ticketMessage.LastUpdate = DateTime.UtcNow;
				await context.SaveChangesAsync().ConfigureAwait(false);

				return new WriterResult(true, $"Message ${messageId} Edited");
			}
		}
	}
}
