using System.Threading.Tasks;
using System.Web.Mvc;
using Cryptopia.Admin.Common.Approval;
using Cryptopia.Infrastructure.Common.DataTables;

namespace Web.Admin.Controllers
{
	[Authorize(Roles = "Admin")]
	public class ApprovalController : BaseController
	{
		public IAdminApprovalReader AdminApprovalReader { get; set; }

		public ActionResult Index()
		{
			return View();
		}

		[HttpPost]
		public async Task<ActionResult> GetApprovals(DataTablesModel model)
		{
			return DataTable(await AdminApprovalReader.GetApprovals(model));
		}
	}
}