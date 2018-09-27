using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cryptopia.Common.DataContext;
using Cryptopia.Infrastructure.Common.DataContext;
using Cryptopia.Infrastructure.Common.DataTables;
using Cryptopia.Common.Support;

namespace Cryptopia.Core.Support
{
	public class SupportReader : ISupportReader
	{
		public IDataContextFactory DataContextFactory { get; set; }

		public async Task<DataTablesResponse> GetOpenTickets(DataTablesModel model)
		{
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				var query = context.SupportTicket
					.AsNoTracking()
					.Where(t => t.Status != Enums.SupportTicketStatus.Closed)
					.Select(ticket => new
					{
						Id = ticket.Id,
						UserName = ticket.User.UserName,
						Category = ticket.Category,
						Title = ticket.Title,
						Status = ticket.Status,
						LastUpdate = ticket.LastUpdate,
						Opened = ticket.Created
					});
				return await query.GetDataTableResultNoLockAsync(model).ConfigureAwait(false);
			}
		}

		public async Task<DataTablesResponse> GetClosedTickets(DataTablesModel model)
		{
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				var query = context.SupportTicket
					.AsNoTracking()
					.Where(t => t.Status == Enums.SupportTicketStatus.Closed)
					.Select(ticket => new
					{
						Id = ticket.Id,
						UserName = ticket.User.UserName,
						Category = ticket.Category,
						Title = ticket.Title,
						Status = ticket.Status,
						LastUpdate = ticket.LastUpdate,
						Opened = ticket.Created
					});
				return await query.GetDataTableResultNoLockAsync(model).ConfigureAwait(false);
			}
		}

		public async Task<List<SupportTicketModel>> GetUserSupportTickets(string userId)
		{
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				var query = context.SupportTicket
					.AsNoTracking()
					.Where(x => x.UserId == userId)
					.Select(ticket => new SupportTicketModel
					{
						Id = ticket.Id,
						Description = ticket.Description,
						Category = ticket.Category,
						Title = ticket.Title,
						Status = ticket.Status,
						LastUpdate = ticket.LastUpdate,
						Created = ticket.Created
					});
				return await query.ToListNoLockAsync().ConfigureAwait(false);
			}
		}

		public async Task<SupportTicketModel> GetUserSupportTicket(string userId, int ticketId)
		{
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				var query = context.SupportTicket
					.AsNoTracking()
					.Where(x => x.UserId == userId && x.Id == ticketId)
					.Select(ticket => new SupportTicketModel
					{
						Id = ticket.Id,
						Description = ticket.Description,
						Category = ticket.Category,
						Title = ticket.Title,
						Status = ticket.Status,
						LastUpdate = ticket.LastUpdate,
						Created = ticket.Created,
						Messages = ticket.Messages
						.Where(m => !m.IsDraft && !m.IsInternal)
							.OrderByDescending(x => x.Id)
							.Select(m => new SupportTicketMessageModel
							{
								Id = m.Id,
								IsAdminReply = m.SenderId != userId,
								Message = m.Message,
								Sender = m.Sender.UserName,
								TimeStamp = m.Created
							}).ToList()
					});
				return await query.FirstOrDefaultNoLockAsync().ConfigureAwait(false);
			}
		}

	}
}