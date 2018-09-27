using Cryptopia.Common.Currency;
using Cryptopia.Infrastructure.Common.DataTables;
using Cryptopia.Common.Lotto;
using System.Threading.Tasks;
using System.Web.Mvc;
using Web.Site.Extensions;
using Web.Site.Helpers;

namespace Web.Site.Controllers
{
	[Authorize(Roles = "Admin")]
	public class AdminLottoController : BaseController
	{
		public ILottoReader LottoReader { get; set; }
		public ILottoWriter LottoWriter { get; set; }
		public ICurrencyReader CurrencyReader { get; set; }

		[HttpGet]
		[AuthorizeAjax(Roles = "Admin")]
		public ActionResult GetLottoItems()
		{
			return PartialView("_Lotto");
		}

		[HttpPost]
		[AuthorizeAjax(Roles = "Admin")]
		public async Task<ActionResult> GetLottoItems(DataTablesModel param)
		{
			return DataTable(await LottoReader.GetLottoItems(param));
		}


		[HttpGet]
		[AuthorizeAjax(Roles = "Admin")]
		public async Task<ActionResult> CreateLottoItem()
		{
			return View("CreateLottoItemModal", new CreateLottoItemModel
			{
				Currencies = await CurrencyReader.GetCurrencies()
			});
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[AuthorizeAjax(Roles = "Admin")]
		public async Task<ActionResult> CreateLottoItem(CreateLottoItemModel model)
		{
			if (!ModelState.IsValid)
			{
				model.Currencies = await CurrencyReader.GetCurrencies();
				return View("CreateLottoItemModal", model);
			}

			var result = await LottoWriter.CreateLottoItem(model);
			if (!ModelState.IsWriterResultValid(result))
				return View("CreateLottoItemModal", model);

			return CloseModalSuccess(result.Message);
		}

		[HttpGet]
		[AuthorizeAjax(Roles = "Admin")]
		public async Task<ActionResult> UpdateLottoItem(int lottoItemId)
		{
			var lottoModel = await LottoReader.GetLottoItem(lottoItemId);
			return View("UpdateLottoItemModal", new UpdateLottoItemModel
			{
				LottoItemId = lottoModel.LottoItemId,
				Description = lottoModel.Description,
				LottoType = lottoModel.LottoType,
				Status = lottoModel.Status,
				Name = lottoModel.Name
			});
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[AuthorizeAjax(Roles = "Admin")]
		public async Task<ActionResult> UpdateLottoItem(UpdateLottoItemModel model)
		{
			if (!ModelState.IsValid)
				return View("UpdateLottoItemModal", model);

			var result = await LottoWriter.UpdateLottoItem(model);
			if (!ModelState.IsWriterResultValid(result))
				return View("UpdateLottoItemModal", model);

			return CloseModalSuccess(result.Message);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[AuthorizeAjax(Roles = "Admin")]
		public async Task<ActionResult> DeleteLottoItem(int lottoItemId)
		{
			var result = await LottoWriter.DeleteLottoItem(lottoItemId);
			if (!ModelState.IsWriterResultValid(result))
				return JsonError(result.Message);

			return JsonSuccess(result.Message);
		}
	}
}