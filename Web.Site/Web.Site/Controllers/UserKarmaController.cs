using Cryptopia.Infrastructure.Common.DataTables;
using Cryptopia.Common.Karma;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Web.Site.Controllers
{
	public class UserKarmaController : BaseController
	{
		public IKarmaReader KarmaReader { get; set; }

		[HttpGet]
		[Authorize]
		public async Task<ActionResult> GetKarma()
		{
			var model = await KarmaReader.GetUserKarma(User.Identity.GetUserId());
			return PartialView("_Karma", model);
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> GetKarmaHistory(DataTablesModel model)
		{
			return DataTable(await KarmaReader.GetKarmaHistory(User.Identity.GetUserId(), model));
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> GetKarmaReceived(DataTablesModel model)
		{
			return DataTable(await KarmaReader.GetKarmaReceived(User.Identity.GetUserId(), model));
		}
	}
}