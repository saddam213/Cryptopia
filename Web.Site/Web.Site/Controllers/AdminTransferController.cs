using Cryptopia.Infrastructure.Common.DataTables;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using System.Web.Mvc;
using Cryptopia.Common.Transfer;
using Web.Site.Extensions;

namespace Web.Site.Controllers
{
	public class AdminTransferController : BaseController
	{
		public ITransferReader TransferReader { get; set; }

		[HttpGet]
		[AuthorizeAjax(Roles = "Admin, Moderator")]
		public ActionResult GetTransfers()
		{
			return PartialView("_Transfers");
		}

		[HttpPost]
		[AuthorizeAjax(Roles = "Admin, Moderator")]
		public async Task<ActionResult> GetTransfers(DataTablesModel model)
		{
			return DataTable(await TransferReader.AdminGetTransfers(User.Identity.GetUserId(), model));
		}
	}
}