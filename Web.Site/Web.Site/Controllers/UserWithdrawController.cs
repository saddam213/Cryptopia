using System.Threading.Tasks;
using System.Web.Mvc;
using Cryptopia.Infrastructure.Common.DataTables;
using Cryptopia.Common.User;
using Microsoft.AspNet.Identity;
using System;

namespace Web.Site.Controllers
{
	public class UserWithdrawController : BaseController
	{
		public IUserWithdrawReader UserWithdrawReader { get; set; }
		public IUserWithdrawWriter UserWithdrawWriter { get; set; }

		#region Withdrawals

		[HttpGet]
		[Authorize]
		public ActionResult GetWithdrawals()
		{
			return PartialView("_Withdrawals");
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> GetWithdrawals(DataTablesModel model)
		{
			model.iDisplayLength = Math.Min(model.iDisplayLength, 25000);
			return DataTable(await UserWithdrawReader.GetDataTable(User.Identity.GetUserId(), model));
		}

		#endregion
	}
}