namespace Web.Admin.Controllers
{
	using System.Threading.Tasks;
	using System.Web.Mvc;
	using Cryptopia.Admin.Common.Support;
	using Cryptopia.Enums;
	using Cryptopia.Infrastructure.Common.Results;
	using Microsoft.AspNet.Identity;
	using Hubs;

	[Authorize(Roles = "Admin")]
	public class SupportController : BaseUserController
	{
		public ISupportReader SupportReader { get; set; }
		public ITicketWriter TicketWriter { get; set; }
		public ITicketMessageWriter TicketMessageWriter { get; set; }

		private Microsoft.AspNet.SignalR.IHubContext SupportHubContext { get; set; }

		public SupportController() {
			if (SupportHubContext == null)
				SupportHubContext = Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager.GetHubContext<SupportHub>();
		}

		[HttpGet]
		public async Task<ActionResult> Index()
		{
			var queues = await SupportReader.GetSupportQueues();
			var model = new TicketListViewModel
			{
				Queues = queues
			};

			return View("TicketList", model);
		}

		public async Task<ActionResult> GetTicketDetails(int ticketId)
		{
			var model = await SupportReader.GetTicket(ticketId);
			return Json(model);
		}

		[HttpGet]
		public async Task<ActionResult> TicketDetails(int id)
		{
			var model = await SupportReader.GetTicket(id);
			return View("TicketDetails", model);
		}

		[HttpGet]
		public async Task<ActionResult> UpdateTicket(int id)
		{
			var model = await SupportReader.GetUpdateTicket(id);
			return View("UpdateTicketModal", model);
		}

		[HttpPost]
		public async Task<ActionResult> GetOpenTickets(GetTicketsRequestModel model)
		{
			var result = DataTable(await SupportReader.GetTickets(model));
			return result;
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> UpdateTicket(UpdateTicketModel model)
		{
			var result = await TicketWriter.UpdateTicket(User.Identity.GetUserId(), model);
			await SupportHubContext.Clients.All.TicketUpdated(model.TicketId);
			return CloseModalSuccess(result.Message);
		}

		[HttpPost]
		public async Task<ActionResult> MoveTicket(int ticketId, int queueId)
		{
			var result = await TicketWriter.MoveTicket(User.Identity.GetUserId(), ticketId, queueId);
			await SupportHubContext.Clients.All.TicketUpdated(ticketId);
			return Json(result);
		}

		[HttpPost]
		public async Task<ActionResult> CreateMessage(int ticketId, string message, bool isAdminMessage = false)
		{
			var adminUserId = User.Identity.GetUserId();

			IWriterResult result;
			if (isAdminMessage)
				result = await TicketMessageWriter.CreateAdminMessage(ticketId, adminUserId, message);
			else
				result = await TicketMessageWriter.CreateMessage(ticketId, adminUserId, message);

			await SupportHubContext.Clients.All.TicketUpdated(ticketId);
			return Json(result);
		}

		[HttpPost]
		public async Task<ActionResult> DeleteMessage(int messageId, int ticketId)
		{
			var adminUserId = User.Identity.GetUserId();
			var result = await TicketMessageWriter.DeleteMessage(messageId, adminUserId);

			await SupportHubContext.Clients.All.TicketUpdated(ticketId);
			return Json(result);
		}

		[HttpPost]
		public async Task<ActionResult> PublishMessage(int messageId, int ticketId)
		{
			var adminUserId = User.Identity.GetUserId();
			var result = await TicketMessageWriter.PublishMessage(messageId, adminUserId);
			var model = result.Result;
			await SendEmailAsync(EmailTemplateType.SupportNewAdminReply, model.TicketId, model.Email, model.UserId, model.UserName, model.TicketId);

			await SupportHubContext.Clients.All.TicketUpdated(ticketId);
			return Json(result);
		}

		[HttpPost]
		public async Task<ActionResult> EditMessage(int messageId, int ticketId, string message)
		{
			var adminUserId = User.Identity.GetUserId();
			var result = await TicketMessageWriter.EditMessage(messageId, adminUserId, message);

			await SupportHubContext.Clients.All.TicketUpdated(ticketId);
			return Json(result);
		}

		[HttpPost]
		public async Task<ActionResult> CloseTicket(int ticketId)
		{
			var result = await TicketWriter.UpdateTicketStatus(User.Identity.GetUserId(), ticketId, SupportTicketStatus.Closed);

			await SupportHubContext.Clients.All.TicketUpdated(ticketId);
			return Json(result);
		}

		[HttpPost]
		public async Task<ActionResult> ReopenTicket(int ticketId)
		{
			var result = await TicketWriter.UpdateTicketStatus(User.Identity.GetUserId(), ticketId, SupportTicketStatus.Reopened);

			await SupportHubContext.Clients.All.TicketUpdated(ticketId);
			return Json(result);
		}

	}
}