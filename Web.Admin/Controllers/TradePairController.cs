using Cryptopia.Admin.Common.AdminCurrency;
using Cryptopia.Common.TradePair;
using Cryptopia.Infrastructure.Common.DataTables;
using System.Threading.Tasks;
using System.Web.Mvc;
using Web.Admin.Helpers;
using Web.Admin.Models;
using Microsoft.AspNet.Identity;

namespace Web.Admin.Controllers
{
	[Authorize(Roles = "Admin")]
	public class TradePairController : BaseController
	{
		public IAdminTradePairReader AdminTradePairReader { get; set; }
		public IAdminTradePairWriter AdminTradePairWriter { get; set; }

		public IAdminCurrencyReader AdminCurrencyReader { get; set; }

		public ActionResult Index()
		{
			return View();
		}

		[HttpPost]
		public async Task<ActionResult> GetTradePairs(DataTablesModel model)
		{
			return DataTable(await AdminTradePairReader.GetTradePairs(model));
		}

		[HttpGet]
		public async Task<ActionResult> UpdateTradePair(int id)
		{
			var model = await AdminTradePairReader.GetTradePair(id);
			if (model == null)
				return ViewMessageModal(new ViewMessageModel(ViewMessageType.Danger, "Error", $"TradePair '{id}' not found"));

			return View("UpdateTradePairModal", model);
		}

		[HttpGet]
		public async Task<ActionResult> CreateTradePair()
		{
			return View("CreateTradePairModal", new CreateTradePairModel
			{
				Currencies = await AdminCurrencyReader.GetCurrencies()
			});
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> UpdateTradePair(UpdateTradePairModel model)
		{
			if (!ModelState.IsValid)
				return View("UpdateTradePairModal", model);

			var result = await AdminTradePairWriter.UpdateTradePair(User.Identity.GetUserId(), model);
			if (!ModelState.IsWriterResultValid(result))
				return View("UpdateTradePairModal", model);

			return CloseModal(result);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> CreateTradePair(CreateTradePairModel model)
		{
			if (!ModelState.IsValid)
			{
				model.Currencies = await AdminCurrencyReader.GetCurrencies();
				return View("CreateTradePairModal", model);
			}

			var result = await AdminTradePairWriter.CreateTradePair(User.Identity.GetUserId(), model);
			if (!ModelState.IsWriterResultValid(result))
			{
				model.Currencies = await AdminCurrencyReader.GetCurrencies();
				return View("CreateTradePairModal", model);
			}

			return CloseModal(result);
		}
	}
}