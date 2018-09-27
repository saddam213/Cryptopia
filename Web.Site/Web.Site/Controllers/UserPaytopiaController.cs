using Cryptopia.Common.User;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using System.Web.Mvc;
using Cryptopia.Infrastructure.Common.DataTables;
using Cryptopia.Common.Paytopia;
using Web.Site.Extensions;

namespace Web.Site.Controllers
{
	public class UserPaytopiaController : BaseController
	{
		public IPaytopiaReader PaytopiaReader { get; set; }

		#region Payments

		[HttpGet]
		[AuthorizeAjax]
		public ActionResult GetPaytopia()
		{
			return PartialView("_Paytopia");
		}

		[HttpPost]
		[AuthorizeAjax]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> GetPayments(DataTablesModel model)
		{
			return DataTable(await PaytopiaReader.GetPayments(User.Identity.GetUserId(), model));
		}


		[HttpGet]
		[AuthorizeAjax]
		public async Task<ActionResult> GetPayment(int id)
		{
			var item = await PaytopiaReader.GetPayment(User.Identity.GetUserId(), id);
			return View("PaymentInfoModal", item);
		}

		#endregion

	}
}