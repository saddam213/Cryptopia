using Cryptopia.Enums;
using Cryptopia.Infrastructure.Helpers;

namespace Web.Admin.Controllers
{
	using System.Threading.Tasks;
	using System.Web.Mvc;
	using Cryptopia.Infrastructure.Common.DataTables;
	using Web.Admin.Helpers;
	using Web.Admin.Models;
	using Cryptopia.Common.Currency;
	using Cryptopia.Admin.Common.AdminCurrency;
	using Cryptopia.Common.Deposit;
    using Microsoft.AspNet.Identity;

    [Authorize(Roles = "Admin")]
	public class CurrencyController : BaseController
	{
		public IAdminCurrencyReader AdminCurrencyReader { get; set; }
		public IAdminCurrencyWriter AdminCurrencyWriter { get; set; }
		public IDepositService DepositService { get; set; }

		public ActionResult Index()
		{
			return View();
		}

		[HttpPost]
		public async Task<ActionResult> GetCurrencies(DataTablesModel model)
		{
			return DataTable(await AdminCurrencyReader.GetCurrencies(model));
		}

		[HttpPost]
		public async Task<ActionResult> GetCurrency(int currencyId)
		{
			var data = await AdminCurrencyReader.GetCurrency(currencyId);
			return Json(data);
		}

		[HttpGet]
		public async Task<ActionResult> UpdateCurrency(int id)
		{
			var model = await AdminCurrencyReader.GetUpdateCurrency(id);
			if (model == null)
				return ViewMessageModal(new ViewMessageModel(ViewMessageType.Danger, "Error", $"Currency '{id}' not found"));

			return View("UpdateCurrencyModal", model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> UpdateCurrency(UpdateCurrencyModel model)
		{
			if (!ModelState.IsValid)
				return View("UpdateCurrencyModal", model);

			var result = await AdminCurrencyWriter.UpdateCurrency(User.Identity.GetUserId(), model);
			if (!ModelState.IsWriterResultValid(result))
				return View("UpdateCurrencyModal", model);

			return CloseModalSuccess(result.Message);
		}

		[HttpGet]
		public async Task<ActionResult> BeginDelistingCurrency(int id)
		{
			var model = await AdminCurrencyReader.GetUpdateListingStatusModel(id);
			if (model == null)
				return ViewMessageModal(new ViewMessageModel(ViewMessageType.Danger, "Error", $"Currency '{id}' not found"));

			return View("DelistCurrencyModal", model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> BeginDelistingCurrency(UpdateListingStatusModel model)
		{
			if (!ModelState.IsValid)
				return View("DelistCurrencyModal", model);

			if (model.DelistOn == null)
			{
				ModelState.AddModelError(nameof(model.DelistOn), "A delist date is required");
				return View("DelistCurrencyModal", model);
			}

			var isAuthenticated = await CryptopiaAuthenticationHelper.VerifyTwoFactorCode(AuthenticatedFeatureType.Delisting, model.TwoFactorCode);
			if (!isAuthenticated)
			{
				ModelState.AddModelError(nameof(model.TwoFactorCode), "Invalid code");
				return View("DelistCurrencyModal", model);
			}
			
			var result = await AdminCurrencyWriter.BeginDelistingCurrency(User.Identity.GetUserId(), model);

			if (!ModelState.IsWriterResultValid(result))
				return View("DelistCurrencyModal", model);

			return CloseModalSuccess(result.Message);
		}

		[HttpPost]
		public async Task<ActionResult> PingCurrency(int id)
		{
			var result = await DepositService.Ping(id);
			if (result)
				return JsonSuccess("Successfully contacted wallet.");

			return JsonError("No response from wallet.");
		}

		[HttpPost]
		public async Task<ActionResult> GetDeposits(int currencyId, DataTablesModel model)
		{
			return DataTable(await AdminCurrencyReader.GetDeposits(currencyId, model));
		}

		[HttpPost]
		public async Task<ActionResult> GetWithdrawals(int currencyId, DataTablesModel model)
		{
			return DataTable(await AdminCurrencyReader.GetWithdrawals(currencyId, model));
		}

		[HttpPost]
		public async Task<ActionResult> GetTransfers(int currencyId, DataTablesModel model)
		{
			return DataTable(await AdminCurrencyReader.GetTransfers(currencyId, model));
		}

		[HttpPost]
		public async Task<ActionResult> GetAddresses(int currencyId, DataTablesModel model)
		{
			return DataTable(await AdminCurrencyReader.GetAddresses(currencyId, model));
		}
	}
}