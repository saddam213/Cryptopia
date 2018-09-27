using Cryptopia.Common.Balance;
using Cryptopia.Infrastructure.Common.DataTables;
using Cryptopia.Common.TermDeposits;
using Cryptopia.Enums;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using System.Web.Mvc;
using Web.Site.Extensions;
using Web.Site.Helpers;

namespace Web.Site.Controllers
{
	public class UserTermDepositController : BaseController
	{
		public ITermDepositReader TermDepositReader { get; set; }

		[HttpGet]
		[Authorize]
		public async Task<ActionResult> GetTermDeposit()
		{
			return await Task.FromResult(PartialView("_TermDeposit", new UserTermDepositModel
			{
				TermDeposits = new System.Collections.Generic.List<TermDepositItemModel>()
			}));
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> GetPayments(DataTablesModel model)
		{
			return DataTable(await TermDepositReader.GetPayouts(User.Identity.GetUserId(), model));
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> GetClosed(DataTablesModel model)
		{
			return DataTable(await TermDepositReader.GetClosedDeposits(User.Identity.GetUserId(), model));
		}
	}
}