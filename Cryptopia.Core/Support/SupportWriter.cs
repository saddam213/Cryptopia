using Cryptopia.Common.Support;
using System;
using System.Threading.Tasks;
using Cryptopia.Infrastructure.Common.DataContext;
using System.Data.Entity;
using Cryptopia.Common.DataContext;
using Cryptopia.Infrastructure.Common.Results;
using Microsoft.AspNet.Identity;
using Cryptopia.Enums;

namespace Cryptopia.Core.Support
{
	public class SupportWriter : ISupportWriter
	{
		public IDataContextFactory DataContextFactory { get; set; }

		public async Task<IWriterResult> CreateTicket(string userId, CreateSupportTicketModel model)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var ticket = new Entity.Support.SupportTicket
				{
					Category = model.Category,
					Created = DateTime.UtcNow,
					Description = model.Description,
					LastUpdate = DateTime.UtcNow,
					Status = Enums.SupportTicketStatus.New,
					Title = model.Subject,
					UserId = userId,
					QueueId = Constant.DEFAULT_QUEUE_ID
				};

				context.SupportTicket.Add(ticket);
				await context.SaveChangesAsync().ConfigureAwait(false);
				model.TicketId = ticket.Id;
				model.Created = ticket.Created.ToString("M/d/yyyy h:mm:ss tt");
				model.CategoryName = ticket.Category.ToString();
				return new WriterResult(true, $"Successfully created support ticket #{ticket.Id}, a Cryptopia staff member will be in touch shortly.");
			}
		}

		public async Task<IWriterResult> CreateTicketReply(string userId, CreateSupportReplyModel model)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var ticket = await context.SupportTicket
					.Include(t => t.User)
					.FirstOrDefaultNoLockAsync(x => x.UserId == userId && x.Id == model.TicketId).ConfigureAwait(false);
				if (ticket == null)
					return new WriterResult(false, $"Support ticket #{model.TicketId} not found.");

				var reply = new Entity.Support.SupportTicketMessage
				{
					Message = model.Message,
					SenderId = ticket.UserId,
					TicketId = model.TicketId,
					Created = DateTime.UtcNow,
					IsInternal = false,
					LastUpdate = DateTime.UtcNow
				};
				context.SupportTicketMessage.Add(reply);
				ticket.Status = Enums.SupportTicketStatus.UserReply;
				await context.SaveChangesAsync().ConfigureAwait(false);
				model.Sender = ticket.User.UserName;
				model.Timestamp = reply.Created.ToString("M/d/yyyy h:mm:ss tt");
				return new WriterResult(true, $"Successfully added reply to ticket #{ticket.Id}, a Cryptopia staff member will be in touch shortly.");
			}
		}

		public async Task<IWriterResult> UpdateTicketStatus(string userId, UpdateSupportTicketModel model)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var ticket = await context.SupportTicket.FirstOrDefaultNoLockAsync(x => x.Id == model.TicketId).ConfigureAwait(false);
				if (ticket == null)
					return new WriterResult(false, $"Support ticket #{model.TicketId} not found.");

				if (!model.IsAdmin && ticket.UserId != userId)
					return new WriterResult(false, $"Support ticket #{model.TicketId} not found.");

				ticket.Status = model.Status;
				ticket.LastUpdate = DateTime.UtcNow;
				await context.SaveChangesAsync().ConfigureAwait(false);
				model.Timestamp = ticket.LastUpdate.ToString("M/d/yyyy h:mm:ss tt");
				return new WriterResult(true, $"Successfully updated support ticket #{model.TicketId} status to '{model.Status}'");
			}
		}
	}
}
