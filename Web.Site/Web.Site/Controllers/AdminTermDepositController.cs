using Cryptopia.Infrastructure.Common.DataTables;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using System.Web.Mvc;

using Web.Site.Extensions;
using Web.Site.Helpers;
using Web.Site.Models;
using Cryptopia.Common.TermDeposits;
using System;
using Cryptopia.Enums;
using Cryptopia.Common.Address;

namespace Web.Site.Controllers
{
	[Authorize(Roles = "Admin, Moderator")]
	public class AdminTermDepositController : BaseController
	{
		public ITermDepositReader TermDepositReader { get; set; }
		public ITermDepositWriter TermDepositWriter { get; set; }
		public IAddressReader AddressReader { get; set; }


		[HttpGet]
		[AuthorizeAjax(Roles = "Admin, Moderator")]
		public ActionResult GetTermDeposits()
		{
			return PartialView("_TermDeposits");
		}

		[HttpPost]
		[AuthorizeAjax(Roles = "Admin, Moderator")]
		public async Task<ActionResult> GetDeposits(DataTablesModel model)
		{
			return DataTable(await TermDepositReader.AdminGetDeposits(User.Identity.GetUserId(), model));
		}

		[HttpPost]
		[AuthorizeAjax(Roles = "Admin, Moderator")]
		public async Task<ActionResult> GetPayments(DataTablesModel model)
		{
			return DataTable(await TermDepositReader.AdminGetPayouts(User.Identity.GetUserId(), model));
		}

		[HttpGet]
		[AuthorizeAjax(Roles = "Admin")]
		public async Task<ActionResult> UpdatePayment(int id)
		{
			var model = await TermDepositReader.AdminGetPayout(User.Identity.GetUserId(), id);
			if (model == null)
				return ViewMessageModal(new ViewMessageModel(ViewMessageType.Danger, "Error", $"Payment #{id} not found"));

			return View("UpdatePayment", model);
		}

		[HttpPost]
		[AuthorizeAjax(Roles = "Admin")]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> UpdatePayment(UpdateTermDepositPaymentModel model)
		{
			if (!ModelState.IsValid)
				return View("UpdatePayment", model);

			var result = await TermDepositWriter.AdminUpdatePayment(User.Identity.GetUserId(), model);
			if (!ModelState.IsWriterResultValid(result))
				return View("UpdatePayment", model);

			return CloseModalSuccess(result.Message);
		}

		[HttpPost]
		[AuthorizeAjax(Roles = "Admin")]
		public async Task<ActionResult> GenerateAddress(Guid userId)
		{
			var addressResponse = await AddressReader.GetAddress(userId.ToString(), Constant.DOTCOIN_ID);
			if (addressResponse == null)
				return JsonError("Failed to generate address.");
			if (!string.IsNullOrEmpty(addressResponse.ErrorMessage))
				return JsonError(addressResponse.ErrorMessage);

			return JsonSuccess(addressResponse.AddressData);
		}

	}
}