using System.Threading.Tasks;
using System.Web.Mvc;
using Cryptopia.Common.User;
using Microsoft.AspNet.Identity;
using Web.Site.Helpers;
using Cryptopia.Enums;
using System;
using Cryptopia.Common.UserVerification;
using Cryptopia.Infrastructure.Common.DataTables;
using Cryptopia.Common.Balance;

namespace Web.Site.Controllers
{
	public class UserBalanceController : BaseController
	{
		public IBalanceReader BalanceReader { get; set; }
		public IUserBalanceReader UserBalanceReader { get; set; }
		public IUserBalanceWriter UserBalanceWriter { get; set; }
		public IUserSettingsWriter UserSettingsWriter { get; set; }

		#region Balances

		[HttpGet]
		[Authorize]
		public async Task<ActionResult> GetBalances()
		{
			var model = await UserBalanceReader.GetBalances(User.Identity.GetUserId(), true);
			return PartialView("_Balances", model);
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> SetFavorite(int currencyId)
		{
			var result = await UserBalanceWriter.SetFavorite(User.Identity.GetUserId(), currencyId);
			if (!result.Success)
				return JsonError();

			return JsonSuccess();
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> DustBalance(int currencyId)
		{
			var result = await UserBalanceWriter.DustBalance(User.Identity.GetUserId(), currencyId);
			if (!result.Success)
				return JsonError(result.Message);

			return JsonSuccess(result.Message);
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> GetTradePairBalance(int tradePairId)
		{
			return Json(await BalanceReader.GetTradePairBalance(User.Identity.GetUserId(), tradePairId));
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> GetTradeBalance(DataTablesModel param)
		{
			return DataTable(await BalanceReader.GetTradeBalances(User.Identity.GetUserId(), param));
		}

		#endregion
	}
}