using Cryptopia.Infrastructure.Common.DataTables;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using System.Web.Mvc;
using Cryptopia.Common.Currency;
using Cryptopia.Common.Pool;
using Cryptopia.Common.SiteSettings;
using Web.Site.Extensions;
using Web.Site.Helpers;
using Web.Site.Models;
using Cryptopia.Common.TradePair;

namespace Web.Site.Controllers
{
	[Authorize(Roles = "Admin, Moderator")]
	public class AdminTradePairController : BaseController
	{
		public ICurrencyReader CurrencyReader { get; set; }
		public ITradePairReader TradePairReader { get; set; }
		public ITradePairWriter TradePairWriter { get; set; }

		[HttpGet]
		[AuthorizeAjax(Roles = "Admin, Moderator")]
		public ActionResult GetTradePairs()
		{
			return PartialView("_TradePairs");
		}

		[HttpPost]
		[AuthorizeAjax(Roles = "Admin, Moderator")]
		public async Task<ActionResult> GetTradePairs(DataTablesModel model)
		{
			return DataTable(await TradePairReader.AdminGetTradePairs(model));
		}

		[HttpGet]
		[AuthorizeAjax(Roles = "Admin")]
		public async Task<ActionResult> CreateTradePair()
		{
			return View("CreateTradePairModal", new CreateTradePairModel
			{
				 Currencies = await CurrencyReader.GetCurrencies()
			});
		}

		[HttpPost]
		[AuthorizeAjax(Roles = "Admin")]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> CreateTradePair(CreateTradePairModel model)
		{
			if (!ModelState.IsValid)
			{
				model.Currencies = await CurrencyReader.GetCurrencies();
				return View("CreateTradePairModal", model);
			}

			var result = await TradePairWriter.AdminCreateTradePair(model);
			if (!ModelState.IsWriterResultValid(result))
			{
				model.Currencies = await CurrencyReader.GetCurrencies();
				return View("CreateTradePairModal", model);
			}

			return CloseModal(result);
		}


		[HttpGet]
		[AuthorizeAjax(Roles = "Admin")]
		public async Task<ActionResult> UpdateTradePair(int id)
		{
			var model = await TradePairReader.AdminGetTradePair(id);
			if (model == null)
				return ViewMessageModal(new ViewMessageModel(ViewMessageType.Danger, "Error", $"TradePair '{id}' not found"));

			return View("UpdateTradePairModal", model);
		}

		[HttpPost]
		[AuthorizeAjax(Roles = "Admin")]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> UpdateTradePair(UpdateTradePairModel model)
		{
			if (!ModelState.IsValid)
				return View("UpdateTradePairModal", model);

			var result = await TradePairWriter.AdminUpdateTradePair(model);
			if (!ModelState.IsWriterResultValid(result))
				return View("UpdateTradePairModal", model);

			return CloseModal(result);
		}

	}
}