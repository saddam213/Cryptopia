using System.Threading.Tasks;
using System.Web.Mvc;
using Cryptopia.Infrastructure.Common.DataTables;
using Cryptopia.Common.User;
using Microsoft.AspNet.Identity;
using System;

namespace Web.Site.Controllers
{
	public class UserTransferController : BaseController
	{
		public IUserTransferReader UserTransferReader { get; set; }
		public IUserTransferWriter UserTransferWriter { get; set; }

		#region Transfers

		[HttpGet]
		[Authorize]
		public ActionResult GetTransfers()
		{
			return PartialView("_Transfers");
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> GetTransfers(DataTablesModel model)
		{
			model.iDisplayLength = Math.Min(model.iDisplayLength, 25000);
			return DataTable(await UserTransferReader.GetDataTable(User.Identity.GetUserId(), model));
		}

		#endregion
	}
}