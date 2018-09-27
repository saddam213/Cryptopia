using Cryptopia.Infrastructure.Common.DataTables;
using Cryptopia.Common.Paytopia;
using Cryptopia.Common.Shareholder;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using System.Web.Mvc;
using Web.Site.Extensions;

namespace Web.Site.Controllers
{
	public class UserShareholderController : BaseController
	{
		public IPaytopiaReader PaytopiaReader { get; set; }
		public IShareholderReader ShareholderReader { get; set; }
		public IShareholderReader ShareholderWriter { get; set; }

		[HttpGet]
		[Authorize]
		public async Task<ActionResult> GetShareholder()
		{
			var model = await ShareholderReader.GetShareInfo(User.Identity.GetUserId());
			return PartialView("_Placeholder", model);
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> GetPayoutHistory(DataTablesModel model)
		{
			return DataTable(await ShareholderReader.GetPayoutHistory(User.Identity.GetUserId(), model));
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> GetPaytopiaHistory(DataTablesModel model)
		{
			return DataTable(await ShareholderReader.GetPaytopiaHistory(model));
		}

		[HttpGet]
		[AuthorizeAjax]
		public async Task<ActionResult> GetPayment(int id)
		{
			var item = await PaytopiaReader.AdminGetPayment(id);
			return View("PaymentInfoModal", item);
		}
	}
}