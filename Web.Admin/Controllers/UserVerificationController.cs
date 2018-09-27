using System.Web.Mvc;

namespace Web.Admin.Controllers
{
	using System.Threading.Tasks;
	using Cryptopia.Admin.Common.UserVerification;
	using Cryptopia.Infrastructure.Common.DataTables;
    using Microsoft.AspNet.Identity;

    [Authorize(Roles = "Admin")]
	public class UserVerificationController : BaseController
	{

		public IUserVerificationReader UserVerificationReader { get; set; }
		public IUserVerificationWriter UserVerificationWriter { get; set; }

		public ActionResult Index()
		{
			return View();
		}

		public async Task<ActionResult> GetUserDetailsModal(int id)
		{
			var model = await UserVerificationReader.GetUserDetails(id);
			return Json(new
			{
				statusCode = 1,
				html = RenderPartialViewToString("_UserDetailsPartial", model)
			});
		}

		public async Task<ActionResult> GetRejectedUserDetailsModal(int id)
		{
			var model = await UserVerificationReader.GetRejectedUserDetails(id);
			return Json(new
			{
				html = RenderPartialViewToString("_UserDetailsPartial", model)
			});
		}

		public async Task<ActionResult> GetCompletedUserDetailsModal(int id)
		{
			var model = await UserVerificationReader.GetCompletedUserDetails(id);
			return Json(new
			{
				html = RenderPartialViewToString("_UserDetailsPartial", model)
			});
		}

		[HttpPost]
		public async Task<ActionResult> GetUserVerifications(DataTablesModel model)
		{
			return DataTable(await UserVerificationReader.GetUserVerifications(model));
		}

		[HttpPost]
		public async Task<ActionResult> GetRejectedUserVerifications(DataTablesModel model)
		{
			return DataTable(await UserVerificationReader.GetRejectedUserVerifications(model));
		}

		[HttpPost]
		public async Task<ActionResult> GetCompletedUserVerifications(DataTablesModel model)
		{
			return DataTable(await UserVerificationReader.GetCompletedVerifications(model));
		}

		[HttpPost]
		public async Task<ActionResult> RejectUser(int verificationId, string reason)
		{
			var result = await UserVerificationWriter.RejectUser(User.Identity.GetUserId(), verificationId, reason);

			TempData.Add("VerificationActionTitle", "Reject User");
			TempData.Add("VerificationActionSuccess", result.Success);
			TempData.Add("VerificationActionMessage", result.Message);

			return RedirectToAction("Index");
		}

		[HttpPost]
		public async Task<ActionResult> AcceptUser(int verificationId)
		{
			var result = await UserVerificationWriter.AcceptUser(verificationId, User.Identity.GetUserId());

			TempData.Add("VerificationActionTitle", "Accept User");
			TempData.Add("VerificationActionSuccess", result.Success);
			TempData.Add("VerificationActionMessage", result.Message);

			return RedirectToAction("Index");
		}

	}
}