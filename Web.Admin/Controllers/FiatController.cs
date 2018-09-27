using Cryptopia.Admin.Common.Fiat;
using Cryptopia.Infrastructure.Common.DataTables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Web.Admin.Controllers
{
	[Authorize(Roles = "Admin")]
	public class FiatController : BaseController
	{
		public IFiatReader FiatReader { get; set; }

		public ActionResult Index()
		{
			return View();
		}

		[HttpGet]
		public async Task<ActionResult> CreateDeposit()
		{
			return await Task.FromResult(View("DepositFiatModal", new DepositFiatModel
			{
			 
			}));
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> CreateDeposit(DepositFiatModel model)
		{
			if (!ModelState.IsValid)
				return await Task.FromResult(View("DepositFiatModal", model));

			//var result = await AdminUserWriter.ChangeEmail(User.Identity.GetUserId(), model);
			//if (!ModelState.IsWriterResultValid(result))
			//	return View("ChangeEmailModal", model);

			return await Task.FromResult(CloseModalSuccess());
		}



		[HttpGet]
		public async Task<ActionResult> CreateWithdraw()
		{
			return await Task.FromResult(View("WithdrawFiatModal", new WithdrawFiatModel
			{

			}));
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> CreateWithdraw(WithdrawFiatModel model)
		{
			if (!ModelState.IsValid)
				return await Task.FromResult(View("WithdrawFiatModal", model));

			//var result = await AdminUserWriter.ChangeEmail(User.Identity.GetUserId(), model);
			//if (!ModelState.IsWriterResultValid(result))
			//	return View("ChangeEmailModal", model);

			return await Task.FromResult(CloseModalSuccess());
		}





		[HttpPost]
		public async Task<ActionResult> GetDeposits(DataTablesModel model)
		{
			return DataTable(await FiatReader.GetDeposits(model));
		}

		[HttpPost]
		public async Task<ActionResult> GetWithdrawals(DataTablesModel model)
		{
			return DataTable(await FiatReader.GetWithdrawals(model));
		}
	}
}