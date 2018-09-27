using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Cryptopia.Common.Mineshaft;
using Cryptopia.Enums;
using Cryptopia.Common.Pool;
using Cryptopia.Infrastructure.Common.DataTables;
using Web.Site.Models;
using Microsoft.AspNet.Identity;
using Cryptopia.Common.PoolWorker;
using Web.Site.Helpers;
using Web.Site.Extensions;
using Cryptopia.Base;

namespace Web.Site.Controllers
{
	public class MineshaftController : BaseController
	{
		public IPoolReader PoolReader { get; set; }
		public IPoolWorkerReader PoolWorkerReader { get; set; }
		public IMineshaftReader MineshaftReader { get; set; }
		public IMineshaftWriter MineshaftWriter { get; set; }

		[HttpGet]
		[AllowAnonymous]
		public async Task<ActionResult> Index(string pool, AlgoType? algo)
		{
			//var pools = await PoolReader.GetPools();
			//var algos = pools.Select(x => x.AlgoType).Distinct();
			//var currentPool = pools.FirstOrDefault(x => x.AlgoType == algo && x.Symbol == pool);
			//var model = new MineshaftModel
			//{
			//	Algos = algos.ToList(),
			//	Pools = pools,
			//	CurrentPool = currentPool,
			//	BaseAlgo = algo
			//};
			//return View("Mineshaft", model);
			return await Task.FromResult(RedirectToAction("Index", "Home"));
		}

		[HttpGet]
		[AllowAnonymous]
		public async Task<ActionResult> GetMineshaftInfo(int id)
		{
			var model = await MineshaftReader.GetMineshaftInfo(id);
			if (model != null)
				return PartialView("_MineshaftInfo", model);

			return PartialViewMessage(new ViewMessageModel(ViewMessageType.Info, "No Results Found", "No pool found matching your criteria"));
		}

		[HttpGet]
		[Authorize]
		public async Task<ActionResult> GetMineshaftUserInfo(int id)
		{
			return Json(await MineshaftReader.GetMineshaftUserInfo(User.Identity.GetUserId(), id), JsonRequestBehavior.AllowGet);
		}

		[AllowAnonymous]
		public async Task<ActionResult> MineshaftSummary(AlgoType? algoType)
		{
			var data = await MineshaftReader.GetMineshaftSummary();
			data.AlgoType = algoType;
			return PartialView("_MineshaftSummary", data);
		}


		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> GetAlgoSummary(DataTablesModel param, AlgoType? algoType)
		{
			return DataTable(await MineshaftReader.GetMineshaftSummary(param, algoType));
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> GetBlocks(DataTablesModel param, int poolId)
		{
			return DataTable(await PoolReader.GetBlocks(param, poolId));
		}


		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> GetPayouts(DataTablesModel param, int poolId)
		{
			return DataTable(await PoolReader.GetPayouts(User.Identity.GetUserId(), param, poolId));
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> GetWorkers(DataTablesModel param, int poolId)
		{
			return DataTable(await PoolWorkerReader.GetWorkers(poolId, param));
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> GetMiners(DataTablesModel param, int poolId)
		{
			return DataTable(await MineshaftReader.GetMiners(poolId, param));
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> GetUserWorkers(DataTablesModel param)
		{
			return DataTable(await PoolWorkerReader.GetWorkers(User.Identity.GetUserId(), param));
		}


		[HttpGet]
		[AllowAnonymous]
		public async Task<ActionResult> GetHashrateChart(int poolId)
		{
			return Json(await MineshaftReader.GetHashrateChart(poolId, User.Identity.GetUserId()), JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		[AllowAnonymous]
		public async Task<ActionResult> GetBlockChart(int poolId)
		{
			return Json(await MineshaftReader.GetBlockChart(poolId), JsonRequestBehavior.AllowGet);
		}


		[HttpGet]
		[AuthorizeAjax]
		public async Task<ActionResult> ChangeUserPool(int poolId)
		{
			var pool = await PoolReader.GetPool(poolId);
			var workers = await PoolWorkerReader.GetWorkers(User.Identity.GetUserId(), pool.AlgoType);
			if (workers.IsNullOrEmpty())
				return ViewMessageModal(new ViewMessageModel(ViewMessageType.Warning, "No Workers Configured", "You do not have any workers configured, please create a worker in your accounts 'Miners' section"));

			return View("ChangePoolModal", new ChangePoolModel
			{
				PoolId = pool.Id,
				PoolName = pool.Name,
				PoolSymbol = pool.Symbol,
				AlgoType = pool.AlgoType,
				Workers = workers,
				AllWorkers = true,
				SelectedWorkers = new List<int>()
			});
		}

		[HttpPost]
		[AuthorizeAjax]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ChangeUserPool(ChangePoolModel model)
		{
			if (!ModelState.IsValid)
				return await ChangeUserPool(model.PoolId);

			var result = await MineshaftWriter.ChangeUserPool(User.Identity.GetUserId(), model);
			if (!ModelState.IsWriterResultValid(result))
				return View("ChangePoolModal", model);

			return CloseModal(result);
		}


		[HttpGet]
		[AllowAnonymous]
		public async Task<ActionResult> GettingStarted(int poolId)
		{
			var pool = await PoolReader.GetPool(poolId);
			var connection = await PoolReader.GetPoolConnection(pool.AlgoType);
			return View("GettingStartedModal", new GettingStartedModel
			{
				PoolId = pool.Id,
				PoolName = pool.Name,
				PoolSymbol = pool.Symbol,
				AlgoType = pool.AlgoType,
				Port = connection.Port,
				StratumUrl = connection.Host
			});
		}
	}
}