using Cryptopia.Infrastructure.Common.DataTables;
using System.Threading.Tasks;
using System.Web.Mvc;
using Cryptopia.Common.Support;
using Web.Site.Extensions;

namespace Web.Site.Controllers
{
	public class AdminSupportController : BaseController
	{
		public ISupportReader SupportReader { get; set; }

		[HttpGet]
		[AuthorizeAjax(Roles = "Admin, Moderator")]
		public ActionResult GetSupport()
		{
			return PartialView("_Support");
		}

		[HttpPost]
		[AuthorizeAjax(Roles = "Admin, Moderator")]
		public async Task<ActionResult> GetOpenTickets(DataTablesModel model)
		{
			return DataTable(await SupportReader.GetOpenTickets(model));
		}

		[HttpPost]
		[AuthorizeAjax(Roles = "Admin, Moderator")]
		public async Task<ActionResult> GetClosedTickets(DataTablesModel model)
		{
			return DataTable(await SupportReader.GetClosedTickets(model));
		}
	}
}