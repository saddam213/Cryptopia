using Cryptopia.Admin.Common.Support;
using Cryptopia.Entity.Support;
using Cryptopia.Enums;
using Cryptopia.Infrastructure.Common.DataContext;
using Cryptopia.Infrastructure.Common.Results;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptopia.Admin.Core.Support
{
	public class QueueWriter : IQueueWriter
	{
		public IDataContextFactory DataContextFactory { get; set; }

		public async Task<IWriterResult> CreateQueue(string adminUserId, string name)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var highestOrder = await context.SupportTicketQueue.MaxNoLockAsync(q => q.Order).ConfigureAwait(false);

				var queue = new SupportTicketQueue
				{
					Name = name,
					Order = (highestOrder + 10)
				};

				context.SupportTicketQueue.Add(queue);
                context.LogActivity(adminUserId, $"New Queue created: {name}");
				await context.SaveChangesAsync().ConfigureAwait(false);

				return new WriterResult(true, $"Queue Created");
			}
		}

		public async Task<IWriterResult> UpdateQueue(SupportQueueModel model)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var queue = await context.SupportTicketQueue.Where(q => q.Id == model.Id).FirstNoLockAsync().ConfigureAwait(false);
				queue.Name = model.Name;
				queue.Order = model.Order;
				await context.SaveChangesAsync().ConfigureAwait(false);

				return new WriterResult(true, $"Queue Updated");
			}
		}

		public async Task<IWriterResult> DeleteQueue(string adminUserId, SupportQueueModel model)
		{
			if (model.IsDefaultQueue) {
				return new WriterResult(false, $"Cannot Delete Default Queue");
			}

			using (var context = DataContextFactory.CreateContext())
			{
				var projection = await context.SupportTicketQueue.Where(q => q.Id == model.Id)
					.Select(q => new {
						Queue = q,
						HasOpenTickets = q.Tickets.Any(t => t.Status != SupportTicketStatus.Closed)
					}).FirstNoLockAsync().ConfigureAwait(false);

				if (projection.HasOpenTickets) {
					return new WriterResult(false, $"Cannot Delete Queue with open tickets");
				}

                projection.Queue.IsDeleted = true;
                context.LogActivity(adminUserId, $"Queue closed: {model.Name}");
				await context.SaveChangesAsync().ConfigureAwait(false);

				return new WriterResult(true, $"Queue Deleted");
			}
		}


	}
}
