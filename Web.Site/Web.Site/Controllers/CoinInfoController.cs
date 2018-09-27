using System.Threading.Tasks;
using System.Web.Mvc;
using Cryptopia.Common.Currency;
using Cryptopia.Infrastructure.Common.DataTables;
using Web.Site.Models;
using Web.Site.Extensions;
using Web.Site.Helpers;
using System;

namespace Web.Site.Controllers
{
	public class CoinInfoController : BaseController
	{
		public ICurrencyReader CurrencyReader { get; set; }
		public ICurrencyWriter CurrencyWriter { get; set; }

		[HttpGet]
		public ActionResult Index(string coin = "", string view = "info")
		{
			var viewMode = view.Equals("info") ? "" : view;
			var currency = string.IsNullOrEmpty(coin) ? "DOT" : coin;
			return View("CoinInfo", new CoinInfoModel
			{
				Coin = currency,
				View = viewMode
			});
		}

		[HttpGet]
		public async Task<ActionResult> GetCoinInfo(DataTablesModel model)
		{
			return DataTable(await CurrencyReader.GetCurrencyInfo(model));
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> GetCoinSummary(int currencyId)
		{
			return JsonResult(await CurrencyReader.GetCurrencySummary(currencyId));
		}


		[HttpGet]
		[AuthorizeAjax(Roles = "Admin, Moderator, ChatModerator")]
		public async Task<ActionResult> UpdateInfo(int id)
		{
			var model = await CurrencyReader.GetCurrencyInfo(id);
			if (model == null)
			{
				return ViewMessageModal(new ViewMessageModel(ViewMessageType.Danger, Resources.CoinInfo.infoCurrencyNotFoundMessageTitle,
						                String.Format(Resources.CoinInfo.infoCurrencyNotFoundMessage, id)));
			}

			return View("UpdateInfoModal", model);
		}

		[HttpPost]
		[AuthorizeAjax(Roles = "Admin, Moderator, ChatModerator")]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> UpdateInfo(UpdateCurrencyInfoModel model)
		{
			if (!ModelState.IsValid)
				return View("UpdateInfoModal", model);

			var result = await CurrencyWriter.UpdateCurrencyInfo(model);
			if (!ModelState.IsWriterResultValid(result))
				return View("UpdateInfoModal", model);

			return CloseModalSuccess(result.Message);
		}

		[HttpGet]
		public async Task<ActionResult> PeerInfo(int id)
		{
			var model = await CurrencyReader.GetPeerInfo(id);
			if (model == null)
				return ViewMessageModal(new ViewMessageModel(ViewMessageType.Danger, Resources.CoinInfo.infoCurrencyNotFoundMessageTitle, 
				                        String.Format(Resources.CoinInfo.infoCurrencyNotFoundMessage, id)));

			return View("PeerInfoModal", model);
		}
	}
}