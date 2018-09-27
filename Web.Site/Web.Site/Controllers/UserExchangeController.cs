using System.Threading.Tasks;
using System.Web.Mvc;
using Cryptopia.Infrastructure.Common.DataTables;
using Cryptopia.Common.User;
using Microsoft.AspNet.Identity;
using System;

namespace Web.Site.Controllers
{
	public class UserExchangeController : BaseController
	{
		public IUserExchangeReader UserExchangeReader { get; set; }
		public IUserExchangeWriter UserExchangeWriter { get; set; }

		#region Trades

		[HttpGet]
		[Authorize]
		public ActionResult GetTrades()
		{
			return PartialView("_Trades");
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> GetTrades(DataTablesModel model)
		{
			return DataTable(await UserExchangeReader.GetTradeDataTable(User.Identity.GetUserId(), model));
		}

		[HttpGet]
		[Authorize]
		public ActionResult GetTradeHistory()
		{
			return PartialView("_TradeHistory");
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> GetTradeHistory(DataTablesModel model)
		{
			model.iDisplayLength = Math.Min(model.iDisplayLength, 25000);
			return DataTable(await UserExchangeReader.GetTradeHistoryDataTable(User.Identity.GetUserId(), model));
		}

		#endregion
	}
}