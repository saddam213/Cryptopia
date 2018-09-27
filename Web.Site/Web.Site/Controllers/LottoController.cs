using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Cryptopia.Enums;
using Cryptopia.Common.Currency;
using Cryptopia.Common.Trade;
using Cryptopia.Common.Lotto;
using Cryptopia.Common.Balance;
using Cryptopia.Infrastructure.Common.DataTables;
using System.Collections.Generic;
using Web.Site.Notifications;

namespace Web.Site.Controllers
{
	public class LottoController : BaseUserController
	{
		public ILottoReader LottoReader { get; set; }
		public ILottoWriter LottoWriter { get; set; }
		public IBalanceReader BalanceReader { get; set; }
		public ICurrencyReader CurrencyReader { get; set; }
		public ITradeService TradeService { get; set; }

		[HttpGet]
		public async Task<ActionResult> Index()
		{
			return View(new LottoViewModel
			{
				LottoItems = await LottoReader.GetLottoItems(User.Identity.GetUserId())
			});
		}

		[HttpPost]
		public async Task<ActionResult> GetLottoHistory(DataTablesModel param)
		{
			return DataTable(await LottoReader.GetHistory(param));
		}

		[HttpPost]
		[Authorize]
		public async Task<ActionResult> GetUserLottoHistory(DataTablesModel param)
		{
			return DataTable(await LottoReader.GetUserHistory(User.Identity.GetUserId(), param));
		}

		[HttpPost]
		[Authorize]
		public async Task<ActionResult> GetUserLottoTickets(DataTablesModel param)
		{
			return DataTable(await LottoReader.GetUserTickets(User.Identity.GetUserId(), param));
		}

		[HttpGet]
		[Authorize]
		public async Task<ActionResult> CreateLottoTicket(int id)
		{
			var user = await UserManager.FindValidByIdAsync(User.Identity.GetUserId());
			if (user == null)
				return UnauthorizedModal();

			var lottoModel = await LottoReader.GetLottoItem(id);
			if (lottoModel == null)
				return View("~/Views/Modal/Invalid.cshtml");

			var balance = await BalanceReader.GetCurrencyBalance(User.Identity.GetUserId(), lottoModel.CurrencyId);
			if (balance == null)
				return View("~/Views/Modal/Invalid.cshtml");

			var model = user.GetTwoFactorModel<LottoPaymentModel>(TwoFactorComponent.Transfer);
			model.Balance = balance.Available;
			model.Symbol = lottoModel.Symbol;
			model.Name = lottoModel.Name;
			model.Description = lottoModel.Description;
			model.Rate = lottoModel.Rate;
			model.LottoItemId = lottoModel.LottoItemId;
			return View("CreateLottoTicketModal", model);
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> CreateLottoTicket(LottoPaymentModel model)
		{
			if (!ModelState.IsValid)
				return View("CreateLottoTicketModal", model);

			var user = await UserManager.FindValidByIdAsync(User.Identity.GetUserId());
			if (user == null)
				return Unauthorized();

			if (!await UserManager.VerifyUserTwoFactorCodeAsync(TwoFactorComponent.Transfer, user.Id, model.Code1, model.Code2))
			{
				ModelState.AddModelError(Resources.Lotto.lottoCreateTicketErrorTitle, Resources.Lotto.lottoCreateTwoFactorErrorMessage);
				return View("CreateLottoTicketModal", model);
			}

			var response = await TradeService.CreateLotto(User.Identity.GetUserId(), new CreateLottoModel
			{
				LottoItemId = model.LottoItemId,
				EntryCount = model.Amount
			});

			if (response.IsError)
			{
				ModelState.AddModelError(Resources.Lotto.lottoCreateTicketErrorTitle, response.Error);
				return View("CreateLottoTicketModal", model);
			}

			await response.Notifications.SendNotifications();
			return CloseModal(new { Success = true, Message = String.Format(Resources.Lotto.lottoCreateSuccessMessage, model.Amount, model.Name), Tickets = model.Amount });
		}
	}
}