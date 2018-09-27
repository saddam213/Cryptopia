using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Cryptopia.Common.Rewards;
using Cryptopia.Infrastructure.Common.DataTables;

namespace Web.Site.Controllers
{
	public class RewardController : BaseController
	{
		public IRewardReader RewardReader { get; set; }

		public ActionResult Index()
		{
			return View("Reward");
		}

		[HttpPost]
		public async Task<ActionResult> GetRewards(DataTablesModel param)
		{
			return DataTable(await RewardReader.GetHistory(User.Identity.GetUserId(), param));
		}

		[HttpPost]
		public async Task<ActionResult> GetBalances(DataTablesModel param)
		{
			return DataTable(await RewardReader.GetBalances(User.Identity.GetUserId(), param));
		}

		[HttpPost]
		public async Task<ActionResult> GetStatistics(DataTablesModel param)
		{
			return DataTable(await RewardReader.GetStatistics(User.Identity.GetUserId(), param));
		}
	}
}