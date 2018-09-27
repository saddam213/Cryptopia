using Cryptopia.Infrastructure.Common.DataTables;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using System.Web.Mvc;
using Cryptopia.Common.Withdraw;
using Web.Site.Extensions;

namespace Web.Site.Controllers
{
	public class AdminWithdrawController : BaseController
	{
		public IWithdrawReader WithdrawReader { get; set; }

		[HttpGet]
		[AuthorizeAjax(Roles = "Admin, Moderator")]
		public ActionResult GetWithdrawals()
		{
			return PartialView("_Withdrawals");
		}

		[HttpPost]
		[AuthorizeAjax(Roles = "Admin, Moderator")]
		public async Task<ActionResult> GetWithdrawals(DataTablesModel model)
		{
			return DataTable(await WithdrawReader.GetWithdrawals(User.Identity.GetUserId(), model));
		}
	}
}