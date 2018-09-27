using Cryptopia.Common.User;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using System.Web.Mvc;
using Cryptopia.Infrastructure.Common.DataTables;
using System;

namespace Web.Site.Controllers
{
	public class UserDepositController : BaseController
    {
		public IUserDepositReader UserDepositReader { get; set; }
		public IUserDepositWriter UserDepositWriter { get; set; }

		#region Deposits

		[HttpGet]
		[Authorize]
		public ActionResult GetDeposits()
		{
			return PartialView("_Deposits");
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> GetDeposits(DataTablesModel model)
		{
			model.iDisplayLength = Math.Min(model.iDisplayLength, 25000);
			return DataTable(await UserDepositReader.GetDataTable(User.Identity.GetUserId(), model));
		}

		#endregion

	}
}