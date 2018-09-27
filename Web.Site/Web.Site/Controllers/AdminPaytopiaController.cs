using Cryptopia.Infrastructure.Common.DataTables;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using System.Web.Mvc;
using Cryptopia.Common.Deposit;
using Web.Site.Extensions;
using Cryptopia.Common.Paytopia;
using Web.Site.Helpers;

namespace Web.Site.Controllers
{
	public class AdminPaytopiaController : BaseController
	{
		public IPaytopiaReader PaytopiaReader { get; set; }
		public IPaytopiaWriter PaytopiaWriter { get; set; }

		[HttpGet]
		[AuthorizeAjax(Roles = "Admin, Moderator")]
		public ActionResult GetPaytopia()
		{
			return PartialView("_Paytopia");
		}

		[HttpPost]
		[AuthorizeAjax(Roles = "Admin, Moderator")]
		public async Task<ActionResult> GetPayments(DataTablesModel model)
		{
			return DataTable(await PaytopiaReader.AdminGetPayments(model));
		}


		[HttpGet]
		[AuthorizeAjax]
		public async Task<ActionResult> GetPayment(int id)
		{
			var item = await PaytopiaReader.AdminGetPayment(id);
			return View("PaymentInfoModal", item);
		}

		[HttpGet]
		[AuthorizeAjax]
		public async Task<ActionResult> UpdatePayment(int id)
		{
			var item = await PaytopiaReader.AdminGetPayment(id);
			return View("UpdatePaymentModal", new AdminUpdatePaytopiaPaymentModel
			{
				PaymentId = item.Id,
				Status = item.Status,
				Reason = item.RefundReason
			});
		}

		[HttpPost]
		[AuthorizeAjax]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> UpdatePayment(AdminUpdatePaytopiaPaymentModel model)
		{
			if (!ModelState.IsValid)
			{
				return View("UpdatePaymentModal", model);
			}

			var result = await PaytopiaWriter.AdminUpdatePaytopiaPayment(model);
			if (!ModelState.IsWriterResultValid(result))
			{
				return View("UpdatePaymentModal", model);
			}

			return CloseModal(result);
		}
	}
}