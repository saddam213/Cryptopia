namespace Cryptopia.Admin.Core.Support
{
	using System;
	using System.Data.Entity;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Threading.Tasks;
	using Cryptopia.Admin.Common.Support;
	using Cryptopia.Enums;
	using Cryptopia.Infrastructure.Common.DataContext;
	using Cryptopia.Infrastructure.Common.DataTables;
	using System.Collections.Generic;

	public class SupportReader : ISupportReader
	{
		public IDataContextFactory DataContextFactory { get; set; }

		public async Task<DataTablesResponse> GetTickets(GetTicketsRequestModel model)
		{
			var dataTablesModel = model.DataTablesModel.Map();

			Expression<Func<Entity.Support.SupportTicket, bool>> isClosedExpression =
				ticket => (ticket.Status == SupportTicketStatus.Closed) == model.IsClosed;

			Expression<Func<Entity.Support.SupportTicket, bool>> queueExpression =
				ticket => model.QueueId.HasValue ? ticket.Queue.Id == model.QueueId : true;

			Expression<Func<Entity.Support.SupportTicket, bool>> tabSearchExpression =
				ticket => model.TabSearch == null ? true : ticket.Tags.Any(t =>t.Name.Contains(model.TabSearch));

			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				var query = context.SupportTicket
					.AsNoTracking()
					.Where(isClosedExpression)
					.Where(queueExpression)
					.Where(tabSearchExpression)
					.Select(ticket => new
					{
						ticket.Id,
						ticket.User.UserName,
						Category = ticket.Category.ToString(),
						ticket.Title,
						Status = ticket.Status.ToString(),
						ticket.LastUpdate,
						Opened = ticket.Created,
						Tags = ticket.Tags.Select(x => x.Name).ToList(),
						QueueId = ticket.Queue.Id,
						QueueName = ticket.Queue.Name
					});
				return await query.GetDataTableResultObjectNoLockAsync(dataTablesModel).ConfigureAwait(false);
			}
		}

		public async Task<UpdateTicketModel> GetUpdateTicket(int id)
		{
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				var projection = await context.SupportTicket
					.AsNoTracking()
					.Where(t => t.Id == id)
					.Select(t => new {
						Ticket = t,
						Tags = t.Tags
					}).FirstOrDefaultNoLockAsync().ConfigureAwait(false);

				var queueDictionary = await context.SupportTicketQueue.ToDictionaryNoLockAsync(x => x.Id, x => x.Name).ConfigureAwait(false);
				var categoryDictionary = Enum.GetValues(typeof(SupportTicketCategory)).Cast<SupportTicketCategory>().ToDictionary(t => t, t => t.ToString());

				var updateModel = new UpdateTicketModel
				{
					TicketId = projection.Ticket.Id,
					Title = projection.Ticket.Title,
					Description = projection.Ticket.Description,
					QueueId = projection.Ticket.QueueId,
					Category = projection.Ticket.Category,
					Tags = String.Join(",", projection.Tags.Select(t => t.Name)),
					QueueDictionary = queueDictionary,
					CategoryDictionary = categoryDictionary
				};

				return updateModel;
			}
		}

		public async Task<CreateTicketModel> GetCreateTicket()
		{
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				var createModel = new CreateTicketModel
				{
					Category = SupportTicketCategory.General,
					QueueId = Constant.DEFAULT_QUEUE_ID
				};

				createModel.QueueDictionary = await context.SupportTicketQueue.ToDictionaryNoLockAsync(x => x.Id, x => x.Name).ConfigureAwait(false);
				createModel.CategoryDictionary = Enum.GetValues(typeof(SupportTicketCategory)).Cast<SupportTicketCategory>().ToDictionary(t => t, t => t.ToString());

				return createModel;
			}
		}

		public async Task<TicketDetailsViewModel> GetTicket(int id)
		{
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				var viewModel = await context.SupportTicket
					.AsNoTracking()
					.Where(t => t.Id == id)
					.Select(t => new TicketDetailsViewModel
					{
						Ticket = new SupportTicketModel
						{
							Id = t.Id,
							UserName = t.User.UserName,
							Email = t.User.Email,
							Title = t.Title,
							Category = t.Category.ToString(),
							Status = t.Status.ToString(),
							Queue = t.Queue.Name,
							Description = t.Description,
							Created = t.Created,
							LastUpdate = t.LastUpdate,
							Tags = t.Tags.Select(x => x.Name).ToList()
						},
						Messages = t.Messages.Select(m => new SupportMessageModel
						{
							Id = m.Id,
							TicketId = m.TicketId,
							IsInternal = m.IsInternal,
							IsDraft = m.IsDraft,
							Created = m.Created,
							LastUpdate = m.LastUpdate,
							Message = m.Message,
							SenderId = m.SenderId,
							UserName = m.Sender.UserName,
							Email = m.Sender.Email
						}).OrderByDescending(x => x.Created).ToList()
					}).FirstOrDefaultNoLockAsync().ConfigureAwait(false);

				viewModel.Ticket.OpenTickets = await context.SupportTicket
					.AsNoTracking()
					.Where(t => t.User.UserName == viewModel.Ticket.UserName && t.Status != SupportTicketStatus.Closed && t.Id != viewModel.Ticket.Id)
					.Select(t => new SupportTicketBasicInfoModel
					{
						Id = t.Id,
						Title = t.Title,
						Created = t.Created
					}).ToListNoLockAsync().ConfigureAwait(false);

				return viewModel;
			}
		}

		public async Task<List<SupportQueueModel>> GetSupportQueues()
		{
			using (var context = DataContextFactory.CreateReadOnlyContext()) {

				return await context.SupportTicketQueue
                    .Where(x => !x.IsDeleted)
                    .OrderBy(q => q.Order).Select(q => new SupportQueueModel
				    {

					    Id = q.Id,
					    Name = q.Name,
					    Order = q.Order,
					    HasOpenTickets = q.Tickets.Any(t => t.Status != SupportTicketStatus.Closed)

				    }).ToListNoLockAsync().ConfigureAwait(false);
			}
		}
	}
}