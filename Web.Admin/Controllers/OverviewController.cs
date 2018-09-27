using Cryptopia.Admin.Common.AdminCurrency;
using Cryptopia.Infrastructure.Common.DataTables;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Web.Admin.Controllers
{
	[Authorize(Roles = "Admin")]
	public class OverviewController : BaseController
	{
		public IOverviewReader OverviewReader { get; set; }

		public ActionResult Index()
		{
			return View();
		}

		[HttpPost]
		public async Task<ActionResult> GetDeposits(DataTablesModel model)
		{
			return DataTable(await OverviewReader.GetDeposits(model));
		}

		[HttpPost]
		public async Task<ActionResult> GetWithdrawals(DataTablesModel model)
		{
			return DataTable(await OverviewReader.GetWithdrawals(model));
		}

		[HttpPost]
		public async Task<ActionResult> GetTransfers(DataTablesModel model)
		{
			return DataTable(await OverviewReader.GetTransfers(model));
		}

		[HttpPost]
		public async Task<ActionResult> GetAddresses(DataTablesModel model)
		{
			return DataTable(await OverviewReader.GetAddresses(model));
		}

		[HttpPost]
		public async Task<ActionResult> GetLogons(DataTablesModel model)
		{
			return DataTable(await OverviewReader.GetLogons(model));
		}
	}
}