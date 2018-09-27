using Cryptopia.Admin.Common.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace Web.Admin.Controllers
{
	[Authorize(Roles = "Admin")]
	public class SupportManagementController : BaseUserController
	{
		public ISupportReader SupportReader { get; set; }
		public ITicketWriter TicketWriter { get; set; }
		public IQueueWriter QueueWriter { get; set; }

		[HttpGet]
		public async Task<ActionResult> Index()
		{
			var queues = await SupportReader.GetSupportQueues();
			return View(queues);
		}

		[HttpGet]
		public async Task<ActionResult> CreateTicket()
		{
			var model = await SupportReader.GetCreateTicket();
			return View("CreateTicketModal", model);
		}

		[HttpPost]
		public async Task<ActionResult> CreateTicket(CreateTicketModel model)
		{
			if (!ModelState.IsValid)
				return CloseModalError("Error Creating Ticket");

			var result = await TicketWriter.CreateTicket(model);

			if (result.Success)
			{
				return CloseModalSuccess(result.Message);
			}

			return CloseModalError(result.Message);
		}

		[HttpPost]
		public async Task<ActionResult> CreateQueue(string name)
		{
			var result = await QueueWriter.CreateQueue(User.Identity.GetUserId(), name);
			return Json(result);
		}

		[HttpPost]
		public async Task<ActionResult> UpdateQueue(SupportQueueModel model)
		{
			var result = await QueueWriter.UpdateQueue(model);
			return Json(result);
		}

		[HttpPost]
		public async Task<ActionResult> DeleteQueue(SupportQueueModel model)
		{
			var result = await QueueWriter.DeleteQueue(User.Identity.GetUserId(), model);
			return Json(result);
		}

	}
}