using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Web.Site.Models;
using Web.Site.Notifications;
using Microsoft.AspNet.Identity;
using Cryptopia.Common.TradePair;
using Cryptopia.Common.Currency;
using Cryptopia.Enums;
using Cryptopia.Common.Exchange;
using Cryptopia.Infrastructure.Common.DataTables;
using Cryptopia.Common.Trade;
using Web.Site.Helpers;
using Cryptopia.Common.Api;
using System.Web;

namespace Web.Site.Controllers
{
	public class ExchangeController : BaseController
	{
		public ITradePairReader TradePairReader { get; set; }
		public ICurrencyReader CurrencyReader { get; set; }
		public IExchangeReader ExchangeReader { get; set; }
		public ITradeService TradeService { get; set; }

		[HttpGet]
		[AllowAnonymous]
		public async Task<ActionResult> Index(int? id, string market, string basemarket = "BTC")
		{
			var viewModel = new ExchangeModel();
			var baseCurrencies = await CurrencyReader.GetBaseCurrencies();
			var baseMarket = baseCurrencies.FirstOrDefault(x => x.Symbol == basemarket)
										?? baseCurrencies.FirstOrDefault(x => x.Symbol == "BTC");
			var tradePair = id.HasValue
							? await TradePairReader.GetTradePair(id.Value)
							: await TradePairReader.GetTradePair(market);

			viewModel.TradePair = tradePair;
			viewModel.BaseCurrencies = baseCurrencies;
			viewModel.BaseMarket = baseMarket?.Symbol ?? "BTC";
			if(viewModel.TradePair == null)
			{
				viewModel.TradePair = new TradePairModel();
				viewModel.Summary = await ExchangeReader.GetExchangeSummary();
				viewModel.Summary.BaseMarket = baseMarket?.Symbol ?? "BTC";
			}
			
			return View("Exchange", viewModel);
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> GetTradePairData(int tradepairId)
		{
			var ticker = await TradePairReader.GetTicker(tradepairId);
			var tradePair = await TradePairReader.GetTradePair(tradepairId);
			var orderbookData = await ExchangeReader.GetOrderBook(User.Identity.GetUserId(), tradepairId);
			var historyData = await ExchangeReader.GetTradeHistory(tradepairId);
			return Json(new
			{
				Success = true,
				TradePair = tradePair,
				Ticker = ticker,
				Buys = orderbookData.BuyData,
				Sells = orderbookData.SellData,
				History = historyData
			});
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> GetTradePairUserData(int tradepairId)
		{
			var orderData = await ExchangeReader.GetUserOrders(User.Identity.GetUserId(), tradepairId);
			var historyData = await ExchangeReader.GetUserHistory(User.Identity.GetUserId(), tradepairId);
			return Json(new
			{
				Success = true,
				Open = orderData,
				History = historyData
			});
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> GetUserOpenTrades(DataTablesModel param)
		{
			return DataTable(await ExchangeReader.GetUserOrders(param, User.Identity.GetUserId()));
		}


		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> GetCurrencySummary(DataTablesModel param, string baseMarket)
		{
			return DataTable(await ExchangeReader.GetExchangeSummary(param, baseMarket));
		}

		[HttpGet]
		[AllowAnonymous]
		public async Task<ActionResult> GetTradePairChart(int tradePairId, int dataRange, int dataGroup)
		{
			return Json(await ExchangeReader.GetStockChart(tradePairId, dataRange, dataGroup), JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		[AllowAnonymous]
		public async Task<ActionResult> GetCurrencyDistribution(int currencyId, ChartDistributionCount count)
		{
			return Json(await ExchangeReader.GetDistributionChart(currencyId, count), JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> SubmitTrade(SubmitTradeModel model)
		{
			if (!ModelState.IsValid)
				return JsonError(ModelState.FirstError());

			var tradePair = await TradePairReader.GetTradePair(model.TradePairId);
			if (tradePair == null)
				return JsonError(Resources.Exchange.tradeNoMarketError);

			if (model.Amount * model.Price < tradePair.BaseMinTrade)
				return JsonError(string.Format(Resources.Exchange.tradeMinTotalError, $"{tradePair.BaseMinTrade:F8}", tradePair.BaseSymbol));

			if (model.Amount >= 1000000000 || (model.Amount * model.Price) >= 1000000000)
				return JsonError(string.Format(Resources.Exchange.tradeMaxAmountError, "1000000000"));

			if (model.Price < 0.00000001m)
				return JsonError(string.Format(Resources.Exchange.tradeMinPriceError, "0.00000001"));

			if (model.Price > 1000000000)
				return JsonError(string.Format(Resources.Exchange.tradeMaxPriceError, "1000000000"));

			var response = await TradeService.CreateTrade(User.Identity.GetUserId(), new CreateTradeModel
			{
				Amount = model.Amount,
				IsBuy = model.IsBuy,
				Price = model.Price,
				TradePairId = tradePair.TradePairId
			});
			if (response.IsError)
				return JsonError(response.Error);

			await response.Notifications.SendNotifications();
			await response.DataUpdates.SendTradeNotifications();
			return JsonSuccess();
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> CancelTrade(int tradeId, int tradePairId)
		{
			var response = await TradeService.CancelTrade(User.Identity.GetUserId(), new CancelTradeModel
			{
				TradeId = tradeId,
				TradePairId = tradePairId,
				CancelType = CancelTradeType.Trade
			});
			if (response.IsError)
				return JsonError(response.Error);

			await response.Notifications.SendNotifications();
			await response.DataUpdates.SendTradeNotifications();
			return JsonSuccess();
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> CancelAllTrades()
		{
			var response = await TradeService.CancelTrade(User.Identity.GetUserId(), new CancelTradeModel
			{
				CancelType = CancelTradeType.All
			});
			if (response.IsError)
				return JsonError(response.Error);

			await response.Notifications.SendNotifications();
			await response.DataUpdates.SendTradeNotifications();
			return JsonSuccess();
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> CancelTradePairTrades(int tradePairId)
		{
			var response = await TradeService.CancelTrade(User.Identity.GetUserId(), new CancelTradeModel
			{
				TradePairId = tradePairId,
				CancelType = CancelTradeType.TradePair
			});
			if (response.IsError)
				return JsonError(response.Error);

			await response.Notifications.SendNotifications();
			await response.DataUpdates.SendTradeNotifications();
			return JsonSuccess();
		}
	}
}