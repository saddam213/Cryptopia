using Cryptopia.Infrastructure.Common.DataTables;
using Cryptopia.Common.Pool;
using Cryptopia.Common.PoolWorker;
using Cryptopia.Common.Transfer;
using Cryptopia.Enums;
using Microsoft.AspNet.Identity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Web.Site.Extensions;
using Web.Site.Helpers;
using Web.Site.Models;

namespace Web.Site.Controllers
{
	[Authorize(Roles = "Admin, Moderator")]
	public class AdminMineshaftController : BaseController
	{
		public IPoolReader PoolReader { get; set; }
		public IPoolWriter PoolWriter { get; set; }
		public ITransferReader TransferReader { get; set; }
		public IPoolWorkerWriter PoolWorkerWriter { get; set; }


		[HttpGet]
		[AuthorizeAjax(Roles = "Admin, Moderator")]
		public ActionResult GetMineshaft()
		{
			return PartialView("_Mineshaft");
		}

		[HttpGet]
		[AuthorizeAjax(Roles = "Admin, Moderator")]
		public ActionResult GetMineshaftPayments()
		{
			return PartialView("_MineshaftPayments");
		}

		[HttpPost]
		[AuthorizeAjax(Roles = "Admin, Moderator")]
		public async Task<ActionResult> GetPools(DataTablesModel model)
		{
			return DataTable(await PoolReader.AdminGetPools(User.Identity.GetUserId(), model));
		}


		[HttpPost]
		[AuthorizeAjax(Roles = "Admin, Moderator")]
		public async Task<ActionResult> GetConnections(DataTablesModel model)
		{
			return DataTable(await PoolReader.GetPoolConnections(model));
		}

		[HttpPost]
		[AuthorizeAjax(Roles = "Admin, Moderator")]
		public async Task<ActionResult> GetPayments(DataTablesModel model)
		{
			return DataTable(await PoolReader.AdminGetPayouts(User.Identity.GetUserId(), model));
		}

		[HttpGet]
		[AuthorizeAjax(Roles = "Admin")]
		public async Task<ActionResult> UpdatePool(int id)
		{
			var model = await PoolReader.AdminGetPool(id);
			if (model == null)
				return ViewMessageModal(new ViewMessageModel(ViewMessageType.Danger, "Error", $"Payment #{id} not found"));

			return View("UpdatePoolModal", model);
		}

		[HttpPost]
		[AuthorizeAjax(Roles = "Admin")]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> UpdatePool(AdminUpdatePoolModel model)
		{
			if (!ModelState.IsValid)
				return View("UpdatePoolModal", model);

			var result = await PoolWriter.AdminUpdatePool(model);
			if (!ModelState.IsWriterResultValid(result))
				return View("UpdatePoolModal", model);

			return CloseModalSuccess(result.Message);
		}



		[HttpGet]
		[AuthorizeAjax(Roles = "Admin")]
		public async Task<ActionResult> UpdateConnection(AlgoType algoType)
		{
			var model = await PoolReader.GetPoolConnection(algoType);
			if (model == null)
				return ViewMessageModal(new ViewMessageModel(ViewMessageType.Danger, "Error", $"Connection #{algoType} not found"));

			var pools = await PoolReader.GetPools();
			return View("UpdatePoolConnectionModal", new AdminUpdatePoolConnectionModel
			{
				AlgoType = model.AlgoType,
				Host = model.Host,
				Name = model.Name,
				Port = model.Port,
				DefaultDiff = model.DefaultDiff,
				DefaultPool = model.DefaultPool,
				FixedDiffSummary = model.FixedDiffSummary,
				VarDiffHighSummary = model.VarDiffHighSummary,
				VarDiffLowSummary = model.VarDiffLowSummary,
				VarDiffMediumSummary = model.VarDiffMediumSummary,
				Pools = pools.Where(x => x.AlgoType == algoType).OrderBy(x => x.Symbol).ToList()
			});
		}

		[HttpPost]
		[AuthorizeAjax(Roles = "Admin")]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> UpdateConnection(AdminUpdatePoolConnectionModel model)
		{
			if (!ModelState.IsValid)
				return View("UpdatePoolConnectionModal", model);

			var result = await PoolWriter.AdminUpdatePoolConnection(model);
			if (!ModelState.IsWriterResultValid(result))
				return View("UpdatePoolConnectionModal", model);

			return CloseModalSuccess(result.Message);
		}


		[HttpGet]
		[Authorize]
		public async Task<ActionResult> UpdateWorkerPool(AlgoType algoType)
		{
			var pools = await PoolReader.GetPools();
			return View("UpdateWorkerPoolModal", new AdminUpdateWorkerPoolModel
			{
				AlgoType = algoType,
				Pools = pools.Where(x => x.AlgoType == algoType).OrderBy(x => x.Symbol).ToList()
			});
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> UpdateWorkerPool(AdminUpdateWorkerPoolModel model)
		{
			if (!ModelState.IsValid)
				return View("UpdateWorkerPoolModal", model);

			var result = await PoolWorkerWriter.AdminUpdateWorkerPool(model);
			if (!ModelState.IsWriterResultValid(result))
				return View("UpdateWorkerPoolModal", model);

			return CloseModal(result);
		}


		[HttpGet]
		[Authorize]
		public async Task<ActionResult> UpdateSettings()
		{
			var settings = await PoolReader.GetPoolSettings();
			return View("UpdateSettingsModal", new AdminUpdatePoolSettingsModel
			{
				ProcessorEnabled = settings.ProcessorEnabled,
				HashRateCalculationPeriod = settings.HashRateCalculationPeriod,
				StatisticsPollPeriod = settings.StatisticsPollPeriod,
				PayoutPollPeriod = settings.PayoutPollPeriod,
				SitePayoutPollPeriod = settings.SitePayoutPollPeriod,
				ProfitabilityPollPeriod = settings.ProfitabilityPollPeriod,
				ProfitSwitchEnabled = settings.ProfitSwitchEnabled,
				ProfitSwitchDepthBTC = settings.ProfitSwitchDepthBTC,
				ProfitSwitchDepthLTC = settings.ProfitSwitchDepthLTC
			});
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> UpdateSettings(AdminUpdatePoolSettingsModel model)
		{
			if (!ModelState.IsValid)
				return View("UpdateSettingsModal", model);

			var result = await PoolWriter.AdminUpdatePoolSettings(model);
			if (!ModelState.IsWriterResultValid(result))
				return View("UpdateSettingsModal", model);

			return CloseModal(result);
		}
	}
}