using System.Threading.Tasks;
using System.Linq;
using System.Web.Mvc;
using Cryptopia.Infrastructure.Common.DataTables;
using Cryptopia.Common.User;
using Microsoft.AspNet.Identity;
using Web.Site.Helpers;
using Cryptopia.Common.Pool;
using Cryptopia.Enums;
using Cryptopia.Common.PoolWorker;
using Cryptopia.Base.Extensions;
using Cryptopia.Common.Transfer;

namespace Web.Site.Controllers
{
	public class UserMineshaftController : BaseController
	{
		public IPoolReader PoolReader { get; set; }
		public IPoolWorkerReader PoolWorkerReader { get; set; }
		public IPoolWorkerWriter PoolWorkerWriter { get; set; }
		public ITransferReader TransferReader { get; set; }
		public ITransferReader MineshaftReader { get; set; }

		#region MineShaft

		[HttpGet]
		[Authorize]
		public async Task<ActionResult> GetMiners()
		{
			var connections = await PoolReader.GetPoolConnections();
			return PartialView("_Miners", new Cryptopia.Common.Mineshaft.MinersModel
			{
				 Connections = connections
			});
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> GetMiners(DataTablesModel param)
		{
			return DataTable(await PoolWorkerReader.GetWorkers(User.Identity.GetUserId(), param));
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
		public async Task<ActionResult> GetPayouts(DataTablesModel param)
		{
			return DataTable(await PoolReader.GetPayouts(User.Identity.GetUserId(), param));
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> GetTransfers(DataTablesModel param)
		{
			return DataTable(await TransferReader.GetTransfers(User.Identity.GetUserId(), param, new[] { TransferType.Mineshaft}));
		}

		#endregion

		#region Workers

		[HttpGet]
		[Authorize]
		public async Task<ActionResult> CreateWorker()
		{
			var connections = await PoolReader.GetPoolConnections();
			return View("CreateWorkerModal", new PoolWorkerCreateModel
			{
				AlgoTypes = connections.Select(x => x.AlgoType).Distinct().ToList(),
				Connections = connections
			});
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> CreateWorker(PoolWorkerCreateModel model)
		{
			if (!ModelState.IsValid)
			{
				var connections = await PoolReader.GetPoolConnections();
				model.AlgoTypes = connections.Select(x => x.AlgoType).Distinct().ToList();
				model.Connections = connections;
				return View("CreateWorkerModal", model);
			}

			model.FullName = string.Format("{0}.{1}", User.Identity.Name, model.Name);
			model.TargetDifficulty = PoolExtensions.OptionToTargetDifficulty(model.DifficultyOption, model.TargetDifficulty);
			var result = await PoolWorkerWriter.CreateWorker(User.Identity.GetUserId(), model);
			if (!ModelState.IsWriterResultValid(result))
				return View("CreateWorkerModal", model);

			return CloseModal(result);
		}


		[HttpGet]
		[Authorize]
		public async Task<ActionResult> UpdateWorker(int id)
		{
			var worker = await PoolWorkerReader.GetWorker(User.Identity.GetUserId(), id);
			var poolconnection = await PoolReader.GetPoolConnection(worker.AlgoType);
			return View("UpdateWorkerModal", new PoolWorkerUpdateModel
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
				DifficultyOption = PoolExtensions.TargetDifficultyToOption(worker.TargetDifficulty)
			});
		}



		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> UpdateWorker(PoolWorkerUpdateModel model)
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
			var result = await PoolWorkerWriter.UpdateWorker(User.Identity.GetUserId(), model);
			if (!ModelState.IsWriterResultValid(result))
				return View("UpdateWorkerModal", model);

			return CloseModal(result);
		}

		[HttpGet]
		[Authorize]
		public async Task<ActionResult> UpdateWorkerPool(int id)
		{
			var pools = await PoolReader.GetPools();
			var worker = await PoolWorkerReader.GetWorker(User.Identity.GetUserId(), id);
			return View("UpdateWorkerPoolModal", new PoolWorkerUpdatePoolModel
			{
				Id = id,
				Name = worker.Name,
				AlgoType = worker.AlgoType,
				TargetPool = worker.TargetPool,
				Pools = pools.Where(x => x.AlgoType == worker.AlgoType && (x.Status == PoolStatus.OK || x.Status == PoolStatus.Expiring)).OrderBy(x => x.Symbol).ToList()
			});
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> UpdateWorkerPool(PoolWorkerUpdatePoolModel model)
		{
			if (!ModelState.IsValid)
			{
				var pools = await PoolReader.GetPools();
				model.Pools = pools.Where(x => x.AlgoType == model.AlgoType && (x.Status == PoolStatus.OK || x.Status == PoolStatus.Expiring)).OrderBy(x => x.Symbol).ToList();
				return View("UpdateWorkerPoolModal", model);
			}

			var result = await PoolWorkerWriter.UpdateWorkerPool(User.Identity.GetUserId(), model);
			if (!ModelState.IsWriterResultValid(result))
				return View("UpdateWorkerPoolModal", model);

			return CloseModal(result);
		}


		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> DeleteWorker(int workerId)
		{
			var result = await PoolWorkerWriter.DeleteWorker(User.Identity.GetUserId(), workerId);
			if (!result.Success)
				return JsonError(result.Message);

			return JsonSuccess(result.Message);
		}

		#endregion
	}
}