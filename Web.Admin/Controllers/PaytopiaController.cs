using Cryptopia.Admin.Common.Paytopia;
using Cryptopia.Common.Paytopia;
using Cryptopia.Infrastructure.Common.DataTables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Web.Admin.Helpers;
using Microsoft.AspNet.Identity;

namespace Web.Admin.Controllers
{
	[Authorize(Roles = "Admin")]
	public class PaytopiaController : BaseController
	{
		public IAdminPaytopiaService PaytopiaService { get; set; }

		public ActionResult Index()
		{
			return View();
		}

		[HttpPost]
		public async Task<ActionResult> GetPayments(DataTablesModel model)
		{
			return DataTable(await PaytopiaService.GetPayments(model));
		}

		[HttpGet]
		public async Task<ActionResult> GetPayment(int id)
		{
			var item = await PaytopiaService.GetPayment(id);
			return View("PaymentInfoModal", item);
		}

		[HttpGet]
		public async Task<ActionResult> UpdatePayment(int id)
		{
			var item = await PaytopiaService.GetPayment(id);
			return View("UpdatePaymentModal", new AdminUpdatePaytopiaPaymentModel
			{
				PaymentId = item.Id,
				Status = item.Status,
				Reason = item.RefundReason
			});
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> UpdatePayment(AdminUpdatePaytopiaPaymentModel model)
		{
			if (!ModelState.IsValid)
			{
				return View("UpdatePaymentModal", model);
			}

			var result = await PaytopiaService.UpdatePaytopiaPayment(User.Identity.GetUserId(), model);
			if (!ModelState.IsWriterResultValid(result))
			{
				return View("UpdatePaymentModal", model);
			}

			return CloseModal(result);
		}
	}
}