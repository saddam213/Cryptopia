namespace Cryptopia.Admin.Core.Support
{
	using System;
	using System.Linq;
	using System.Threading.Tasks;
	using Cryptopia.Admin.Common.Support;
	using Cryptopia.Enums;
	using Cryptopia.Infrastructure.Common.DataContext;
	using Cryptopia.Infrastructure.Common.Results;
	using Cryptopia.Entity.Support;
	using System.Data.Entity;
	using System.Collections.Generic;

	public class TicketWriter : ITicketWriter
	{
		public IDataContextFactory DataContextFactory { get; set; }

		public async Task<IWriterResult> CreateTicket(CreateTicketModel model)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var user = await context.Users.Where(u => u.UserName == model.UserName).FirstOrDefaultNoLockAsync().ConfigureAwait(false);

				if (user == null)
					return new WriterResult(false, $"User - {model.UserName} not found");

				var ticket = new SupportTicket {
					User = user,
					QueueId = model.QueueId,
					Category = model.Category,
					Title = model.Title,
					Description = model.Description,
					LastUpdate = DateTime.UtcNow,
					Created = DateTime.UtcNow
				};

				context.SupportTicket.Add(ticket);
				await context.SaveChangesAsync().ConfigureAwait(false);

				return new WriterResult(true, $"Ticket - {ticket.Title} Created");
			}
		}

		public async Task<IWriterResult> UpdateTicket(string adminUserId, UpdateTicketModel model)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var ticket = await context.SupportTicket.Include(x => x.Tags).Where(t => t.Id == model.TicketId).FirstNoLockAsync().ConfigureAwait(false);
				var queue = await context.SupportTicketQueue.Where(q => q.Id == model.QueueId).FirstNoLockAsync().ConfigureAwait(false);

				var tagProjections = await context.SupportTag.Select(tag => new {
					Tag = tag,
					TicketCount = tag.Tickets.Count
				}).ToListNoLockAsync().ConfigureAwait(false);

				List<SupportTag> tagToRemoveList;

				if (model.Tags == null) {
					tagToRemoveList = ticket.Tags.ToList();
				}
				else
				{
					var tagNames = model.Tags.Split(',').ToList();
					tagToRemoveList = ticket.Tags.Where(t => !tagNames.Contains(t.Name)).ToList();

					foreach (var tagName in tagNames)
					{
						if (ticket.Tags.Any(x => x.Name == tagName))
						{
							continue;
						}

						var tag = tagProjections.FirstOrDefault(p => p.Tag.Name == tagName)?.Tag;

						if (tag != null)
						{
							ticket.Tags.Add(tag);
						}
						else
						{
							var newTag = new SupportTag
							{
								Name = tagName
							};

							ticket.Tags.Add(newTag);
							context.SupportTag.Add(newTag);
                            context.LogActivity(adminUserId, $"Created new tag: {tagName}");
						}
					}
				}

				foreach (var tagToRemove in tagToRemoveList)
				{
					ticket.Tags.Remove(tagToRemove);
					var tagProjection = tagProjections.First(p => p.Tag.Id == tagToRemove.Id);
					if (tagProjection.TicketCount <= 1)
					{
						context.SupportTag.Remove(tagToRemove);
                        context.LogActivity(adminUserId, $"Removed tag: {tagToRemove.Name}");
                    }
				}

				ticket.Title = model.Title;
				ticket.Description = model.Description;
				ticket.Category = model.Category;
				ticket.Queue = queue;
				ticket.LastUpdate = DateTime.UtcNow;

				await context.SaveChangesAsync().ConfigureAwait(false);

				return new WriterResult(true, $"Ticket - {ticket.Title} updated");
			}
		}

		public async Task<IWriterResult> MoveTicket(string adminUserId, int ticketId, int queueId)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var ticket = await context.SupportTicket.Where(t => t.Id == ticketId).FirstNoLockAsync();
				var queue = await context.SupportTicketQueue.Where(q => q.Id == queueId).FirstNoLockAsync();
                var oldQueue = ticket.Queue;
                ticket.Queue = queue;
				ticket.LastUpdate = DateTime.UtcNow;
                context.LogActivity(adminUserId, $"Moved ticket from Queue: {oldQueue.Name} to {queue.Name}");
				await context.SaveChangesAsync().ConfigureAwait(false);

				return new WriterResult(true, $"Ticket - {ticket.Title} moved to Queue ${queue.Name} (id: {queue.Id})");
			}
		}

		public async Task<IWriterResult> UpdateTicketStatus(string adminUserId, int ticketId, SupportTicketStatus status)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var ticket = await context.SupportTicket.Where(t => t.Id == ticketId).FirstNoLockAsync();
                var oldStatus = ticket.Status;
				ticket.Status = status;
				ticket.LastUpdate = DateTime.UtcNow;
                context.LogActivity(adminUserId, $"Updated ticket status from {oldStatus} to {status}");
				await context.SaveChangesAsync().ConfigureAwait(false);

				return new WriterResult(true, $"Ticket - {ticket.Title} status change to {status}");
			}
		}
	}
}
