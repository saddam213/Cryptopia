using Cryptopia.Infrastructure.Common.DataTables;
using System.Threading.Tasks;
using System.Web.Mvc;
using Cryptopia.Common.Currency;
using Web.Site.Extensions;
using Web.Site.Helpers;
using Web.Site.Models;
using System.Linq;
using Cryptopia.Admin.Common.AdminCurrency;
using Cryptopia.Common.Deposit;

namespace Web.Site.Controllers
{
	[Authorize(Roles = "Admin, Moderator")]
	public class AdminCurrencyController : BaseController
	{
		public ICurrencyReader CurrencyReader { get; set; }
		public ICurrencyWriter CurrencyWriter { get; set; }
		public IDepositService DepositService { get; set; }
		public IAdminCurrencyReader AdminCurrencyReader { get; set; }

		[HttpGet]
		[AuthorizeAjax(Roles = "Admin, Moderator")]
		public async Task<ActionResult> GetCurrencies()
		{
			var currencies = await CurrencyReader.GetCurrencies();
			var model = new AdminCurrencyModel
			{
				Currencies = currencies.Select(x => x.Symbol).OrderBy(x => x).ToList()
			};
			return PartialView("_Currencies", model);
		}

		[HttpPost]
		[AuthorizeAjax(Roles = "Admin")]
		public async Task<ActionResult> GetCurrency(string symbol)
		{
			var currency = await AdminCurrencyReader.GetCurrency(symbol);
			return Json(currency);
		}

		[HttpPost]
		[AuthorizeAjax(Roles = "Admin")]
		public async Task<ActionResult> PingCurrency(int id)
		{
			var result = await DepositService.Ping(id);
			if (result)
				return JsonSuccess("Successfully contacted wallet.");

			return JsonError("No response from wallet.");
		}


		[HttpPost]
		[AuthorizeAjax(Roles = "Admin, Moderator")]
		public async Task<ActionResult> GetCurrencies(DataTablesModel model)
		{
			return DataTable(await CurrencyReader.GetCurrencyDataTable(model));
		}

		[HttpGet]
		[AuthorizeAjax(Roles = "Admin")]
		public async Task<ActionResult> UpdateCurrency(int id)
		{
			var model = await CurrencyReader.GetUpdateCurrency(id);
			if (model == null)
				return ViewMessageModal(new ViewMessageModel(ViewMessageType.Danger, "Error", $"Currency '{id}' not found"));

			return View("UpdateCurrencyModal", model);
		}

		[HttpPost]
		[AuthorizeAjax(Roles = "Admin")]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> UpdateCurrency(UpdateCurrencyModel model)
		{
			if (!ModelState.IsValid)
				return View("UpdateCurrencyModal", model);

			var result = await CurrencyWriter.UpdateCurrency(model);
			if (!ModelState.IsWriterResultValid(result))
				return View("UpdateCurrencyModal", model);

			return CloseModalSuccess(result.Message);
		}
	}
}