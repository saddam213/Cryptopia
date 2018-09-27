using Cryptopia.Common.User;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using System.Web.Mvc;
using Web.Site.Helpers;

namespace Web.Site.Controllers
{
	public class UserMessageController : BaseController
	{
		public IUserMessageReader UserMessageReader { get; set; }
		public IUserMessageWriter UserMessageWriter { get; set; }

		[HttpGet]
		[Authorize]
		public async Task<ActionResult> GetMessages()
		{
			var messages = await UserMessageReader.GetMessages(User.Identity.GetUserId());
			return PartialView("_Messages", new UserMessagesModel
			{
				Messages = messages
			});
		}

		[HttpGet]
		[Authorize]
		public async Task<ActionResult> GetMessage(int messageId)
		{
			var message = await UserMessageReader.GetMessage(User.Identity.GetUserId(), messageId);
			return PartialView("_MessageItem", message);
		}

		[HttpGet]
		[Authorize]
		public ActionResult CreateMessage()
		{
			return View("CreateMessageModal", new UserMessageCreateModel());
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> CreateMessage(UserMessageCreateModel model)
		{
			if (!ModelState.IsValid)
				return View("CreateMessageModal", model);

			var result = await UserMessageWriter.CreateMessage(User.Identity.GetUserId(), model);
			if (!ModelState.IsWriterResultValid(result))
			{
				return View("CreateMessageModal", model);
			}

			return CloseModal(result);
		}

		[HttpGet]
		[Authorize]
		public async Task<ActionResult> ReplyMessage(int messageId)
		{
			var message = await UserMessageReader.GetMessage(User.Identity.GetUserId(), messageId);
			if (message == null)
				return RedirectToRoute("Messages");

			return View("ReplyMessageModal", new UserMessageCreateModel
			{
				Subject = string.Format("{0}{1}", message.Subject.StartsWith("Re:") ? "" : "Re: ", message.Subject),
				Recipiants = message.Sender,
				Message = string.Format("<p></p><br/><br/><hr />{0}", message.Message)
			});
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ReplyMessage(UserMessageCreateModel model)
		{
			if (!ModelState.IsValid)
				return View("ReplyMessageModal", model);

			var result = await UserMessageWriter.CreateMessage(User.Identity.GetUserId(), model);
			if (!result.Success)
			{
				ModelState.AddModelError("", result.Message);
				return View("ReplyMessageModal", model);
			}

			return CloseModal(result);
		}

		[HttpGet]
		[Authorize]
		public ActionResult ReportMessage(int messageId)
		{
			return View("ReportMessageModal", new UserMessageReportModel {MessageId = messageId});
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ReportMessage(UserMessageReportModel model)
		{
			if (!ModelState.IsValid)
				return View("ReportMessageModal", model);

			var result = await UserMessageWriter.ReportMessage(User.Identity.GetUserId(), model);
			return CloseModal(result);
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> DeleteMessage(int messageId)
		{
			var result = await UserMessageWriter.DeleteMessage(User.Identity.GetUserId(), messageId);
			return Json(result);
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> DeleteAllMessage(bool inbox)
		{
			var result = await UserMessageWriter.DeleteAllMessage(User.Identity.GetUserId(), inbox);
			return Json(result);
		}
	}
}