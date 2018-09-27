using System.Threading.Tasks;
using System.Web.Mvc;
using Cryptopia.Infrastructure.Common.DataTables;
using Cryptopia.Common.User;
using Microsoft.AspNet.Identity;

namespace Web.Site.Controllers
{
	public class UserMarketplaceController : BaseController
	{
		public IUserMarketplaceReader UserMarketplaceReader { get; set; }
		public IUserMarketplaceWriter UserMarketplaceWriter { get; set; }

		[HttpGet]
		[Authorize]
		public ActionResult GetMarketItems()
		{
			return PartialView("_MarketItems");
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> GetMarketItems(DataTablesModel param)
		{
			return DataTable(await UserMarketplaceReader.GetMarketItems(User.Identity.GetUserId(), param));
		}


		[HttpGet]
		[Authorize]
		public ActionResult GetHistory()
		{
			return PartialView("_History");
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> GetHistory(DataTablesModel param)
		{
			return DataTable(await UserMarketplaceReader.GetMarketHistory(User.Identity.GetUserId(), param));
		}
	}
}