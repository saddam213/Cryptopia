using Cryptopia.Infrastructure.Common.DataTables;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using System.Web.Mvc;
using Cryptopia.Common.Deposit;
using Web.Site.Extensions;
using Cryptopia.Common.PoolWorker;
using Cryptopia.Common.Pool;
using Web.Site.Helpers;
using Cryptopia.Base.Extensions;
using System.Linq;
using Cryptopia.Enums;

namespace Web.Site.Controllers
{
	public class AdminPoolWorkerController : BaseController
	{
		public IPoolReader PoolReader { get; set; }
		public IPoolWriter PoolWriter { get; set; }
		public IPoolWorkerReader PoolWorkerReader { get; set; }
		public IPoolWorkerWriter PoolWorkerWriter { get; set; }

		[HttpGet]
		[AuthorizeAjax(Roles = "Admin, Moderator")]
		public ActionResult GetPoolWorkers()
		{
			return PartialView("_PoolWorker");
		}

		[HttpPost]
		[AuthorizeAjax(Roles = "Admin, Moderator")]
		public async Task<ActionResult> GetWorkers(DataTablesModel model)
		{
			return DataTable(await PoolWorkerReader.AdminGetWorkers(User.Identity.GetUserId(), model));
		}


		[HttpGet]
		[Authorize]
		public async Task<ActionResult> UpdateWorker(int id)
		{
			var pools = await PoolReader.GetPools();
			var worker = await PoolWorkerReader.AdminGetWorker(User.Identity.GetUserId(), id);
			var poolconnection = await PoolReader.GetPoolConnection(worker.AlgoType);
			return View("UpdateWorkerModal", new AdminPoolWorkerUpdateModel
			{
				Id = id,
				Name = worker.Name,
				AlgoType = worker.AlgoType,
				IsAutoSwitch = worker.IsAutoSwitch,
				Password = worker.Password,
				TargetDifficulty = worker.TargetDifficulty,
				DefaultDiff = poolconnection.DefaultDiff,
				FixedDiffSummary = poolconnection.FixedDiffSummary,
				VarDiffHighSummary = poolconnection.VarDiffHighSummary,
				VarDiffLowSummary = poolconnection.VarDiffLowSummary,
				VarDiffMediumSummary = poolconnection.VarDiffMediumSummary,
				DifficultyOption = PoolExtensions.TargetDifficultyToOption(worker.TargetDifficulty),
				TargetPool = worker.TargetPool,
				Pools = pools.Where(x => x.AlgoType == worker.AlgoType).OrderBy(x => x.Symbol).ToList()
			});
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> UpdateWorker(AdminPoolWorkerUpdateModel model)
		{
			if (!ModelState.IsValid)
			{
				var poolconnection = await PoolReader.GetPoolConnection(model.AlgoType);
				model.FixedDiffSummary = poolconnection.FixedDiffSummary;
				model.VarDiffHighSummary = poolconnection.VarDiffHighSummary;
				model.VarDiffLowSummary = poolconnection.VarDiffLowSummary;
				model.VarDiffMediumSummary = poolconnection.VarDiffMediumSummary;
				return View("UpdateWorkerModal", model);
			}

			model.TargetDifficulty = PoolExtensions.OptionToTargetDifficulty(model.DifficultyOption, model.TargetDifficulty);
			var result = await PoolWorkerWriter.AdminUpdateWorker(model);
			if (!ModelState.IsWriterResultValid(result))
				return View("UpdateWorkerModal", model);

			return CloseModal(result);
		}
	}
}