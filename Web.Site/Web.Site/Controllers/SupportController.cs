using Cryptopia.Common.Support;
using Cryptopia.Enums;
using hbehr.recaptcha;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using System.Web.Mvc;
using Cryptopia.Infrastructure.Helpers;
using Web.Site.Helpers;
using Web.Site.Models;

namespace Web.Site.Controllers
{
	public class SupportController : BaseUserController
	{
		public ISupportReader SupportReader { get; set; }
		public ISupportWriter SupportWriter { get; set; }

		[HttpGet]
		[AllowAnonymous]
		public ActionResult Support()
		{
			if (User.Identity.IsAuthenticated)
			{
				return RedirectToAction("UserSupport");
			}
			return View(new SupportModel());
		}

		[HttpGet]
		[Authorize]
		public async Task<ActionResult> UserSupport()
		{
			var response = await SupportReader.GetUserSupportTickets(User.Identity.GetUserId());
			var model = new UserSupportModel
			{
				SupportTickets = response
			};
			return View(model);
		}

		[HttpGet]
		[Authorize]
		public async Task<ActionResult> SupportTicket(int ticketId)
		{
			var response = await SupportReader.GetUserSupportTicket(User.Identity.GetUserId(), ticketId);
			if (response == null)
				return ViewMessage(new ViewMessageModel(ViewMessageType.Warning, Resources.Support.tickectNotFoundMessageTitle, Resources.Support.tickectNotFoundMessage));

			return View(response);
		}

		[HttpGet]
		[Authorize]
		public ActionResult CreateTicket()
		{
			return View("CreateTicketModal", new CreateSupportTicketModel());
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> CreateTicket(CreateSupportTicketModel model)
		{
			if (!ModelState.IsValid)
				return View("CreateTicketModal", model);

			var result = await SupportWriter.CreateTicket(User.Identity.GetUserId(), model);
			if (!ModelState.IsWriterResultValid(result))
				return View("CreateTicketModal", model);

			return CloseModal(new
			{
				Success = true,
				Message = result.Message,
				Data = model
			});
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> CloseTicket(int id)
		{
			var model = new UpdateSupportTicketModel
			{
				TicketId = id,
				Status = SupportTicketStatus.Closed
			};

			var result = await SupportWriter.UpdateTicketStatus(User.Identity.GetUserId(), model);
			if (!result.Success)
				return JsonError(result.Message);

			return Json(new
			{
				Success = true,
				Message = result.Message,
				Data = model
			});
		}

		[HttpGet]
		[Authorize]
		public ActionResult CreateReply(int ticketId)
		{
			return View("CreateReplyModal", new CreateSupportReplyModel { TicketId = ticketId });
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> CreateReply(CreateSupportReplyModel model)
		{
			if (!ModelState.IsValid)
				return View("CreateReplyModal", model);

			var result = await SupportWriter.CreateTicketReply(User.Identity.GetUserId(), model);
			if (!ModelState.IsWriterResultValid(result))
				return View("CreateReplyModal", model);

			return CloseModal(new
			{
				Success = true,
				Message = result.Message,
				Data = model
			});
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> SendSupportRequest(SupportModel model)
		{
			if (!ModelState.IsValid)
				return View("Support", model);

			if (!CryptopiaAuthenticationHelper.ValidateCaptcha())
			{
				ModelState.AddModelError("", Resources.Support.supportReCaptchaError);
				return View("Support", model);
			}

			if (!await SendSystemEmailAsync(EmailTemplateType.SupportRequest, SystemEmailType.Email_System, model.Email, model.Subject, model.Message))
			{
				model.IsError = true;
				model.Result = Resources.Support.supportFailedError;
				return View("Support", model);
			}
			ModelState.Clear();
			return View("Support", new SupportModel { Result = Resources.Support.supportSuccessMessage });
		}
	}
}