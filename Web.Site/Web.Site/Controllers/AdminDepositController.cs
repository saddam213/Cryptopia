using Cryptopia.Infrastructure.Common.DataTables;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using System.Web.Mvc;
using Cryptopia.Common.Deposit;
using Web.Site.Extensions;

namespace Web.Site.Controllers
{
	public class AdminDepositController : BaseController
	{
		public IDepositReader DepositReader { get; set; }

		[HttpGet]
		[AuthorizeAjax(Roles = "Admin, Moderator")]
		public ActionResult GetDeposits()
		{
			return PartialView("_Deposits");
		}

		[HttpPost]
		[AuthorizeAjax(Roles = "Admin, Moderator")]
		public async Task<ActionResult> GetDeposits(DataTablesModel model)
		{
			return DataTable(await DepositReader.AdminGetDeposits(User.Identity.GetUserId(), model));
		}
	}
}