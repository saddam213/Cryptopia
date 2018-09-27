using Cryptopia.Common.Address;
using Cryptopia.Common.Balance;
using Cryptopia.Common.Currency;
using Cryptopia.Common.Deposit;
using Cryptopia.Common.Trade;
using Cryptopia.Common.Transfer;
using Cryptopia.Common.User;
using Cryptopia.Common.UserVerification;
using Cryptopia.Common.Utilities;
using Cryptopia.Common.Withdraw;
using Cryptopia.Enums;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Web.Site.Controllers
{
	[Authorize]
	public class TransferController : BaseUserController
	{
		public ICurrencyReader CurrencyReader { get; set; }
		public IBalanceReader BalanceReader { get; set; }
		public ITradeService TradeService { get; set; }
		public ITransferReader TransferReader { get; set; }
		public IUserReader UserReader { get; set; }
		public IUserVerificationReader UserVerificationReader { get; set; }
		public IBalanceEstimationService BalanceEstimationService { get; set; }

		[HttpGet]
		[Authorize]
		public async Task<ActionResult> Index(string returnUrl)
		{
			var currencyData = await CurrencyReader.GetCurrencies();
			return View(new TransferViewModel
			{
				Currencies = currencyData,
				ReturnUrl = GetLocalReturnUrl(returnUrl)
			});
		}

		[HttpGet]
		[Authorize]
		public async Task<ActionResult> Create(string currency, string userName, string returnUrl)
		{
			var user = await UserManager.FindValidByIdAsync(User.Identity.GetUserId());
			var currencyData = await CurrencyReader.GetCurrency(currency);
			if (currencyData == null || user == null)
			{
				ViewBag.Message = String.Format(Resources.UserWallet.transferCurrencyNotFoundError, currency);
				return View("Error");
			}

			if (currencyData.Status == CurrencyStatus.Maintenance || currencyData.Status == CurrencyStatus.Offline)
			{
				ViewBag.HideSupportLink = true;
				ViewBag.SubTitle = String.Format("{0}: {1}", Resources.UserWallet.transferCreateStatusLabel, currencyData.Status);
				ViewBag.Message = $"{Resources.UserWallet.transferCreateErrorMessage} <br /><br /> {Resources.UserWallet.transferCreateStatusLabel}: {currencyData.Status}<br />{Resources.UserWallet.transferCreateReasonLabel}: {(string.IsNullOrEmpty(currencyData.StatusMessage) ? Resources.UserWallet.transferCreateUnknownError : currencyData.StatusMessage)}";
				return View("Error");
			}

			var checkUser = await UserReader.GetUserByName(userName);
			return View(await CreateTransferModel(user, new TransferCurrencyModel
			{
				Name = currencyData.Name,
				Symbol = currencyData.Symbol,
				ReturnUrl = GetLocalReturnUrl(returnUrl),
				UserName = checkUser?.UserName
			}));
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Create(TransferCurrencyModel model)
		{
			var currencyData = await CurrencyReader.GetCurrency(model.Symbol);
			var user = await UserManager.FindValidByIdAsync(User.Identity.GetUserId());
			if (currencyData == null || user == null)
			{
				ViewBag.Message = String.Format(Resources.UserWallet.transferCurrencyNotFoundError, model.Symbol);
				return View("Error");
			}

			// Verify User
			var receivingUser = await UserReader.GetUserByName(model.UserName);
			if(receivingUser == null)
			{
				ModelState.AddModelError("UserName", String.Format(Resources.UserWallet.transferUserNotFoundError, model.UserName));
				return View(await CreateTransferModel(user, model));
			}
			if (receivingUser.UserId.Equals(User.Identity.GetUserId(), StringComparison.OrdinalIgnoreCase))
			{
				ModelState.AddModelError("UserName", Resources.UserWallet.transferUserIgnoredError);
				return View(await CreateTransferModel(user, model));
			}

			// Verify Amount
			if (model.Amount < 0.00000001m)
			{
				ModelState.AddModelError("Amount", String.Format(Resources.UserWallet.transferMinAmountError, "0.00000001", currencyData.Symbol));
				return View(await CreateTransferModel(user, model));
			}

			// Verify Two Factor
			if (!await UserManager.VerifyUserTwoFactorCodeAsync(TwoFactorComponent.Transfer, user.Id, model.Code1, model.Code2))
			{
				ModelState.AddModelError("TwoFactorError", Resources.UserWallet.transferTwoFactorFailedError);
				return View(await CreateTransferModel(user, model));
			}

			// Create transfer
			var result = await TradeService.CreateTransfer(User.Identity.GetUserId(), new CreateTransferModel
			{
				Receiver = receivingUser.UserId,
				Amount = model.Amount,
				CurrencyId = model.CurrencyId,
				TransferType = TransferType.User
			});

			if (result.IsError)
			{
				ModelState.AddModelError("Error", result.Error);
				return View(await CreateTransferModel(user, model));
			}

			return RedirectToAction("Summary", new { transferId = result.TransferId, returnUrl = GetLocalReturnUrl(model.ReturnUrl) });
		}

		[HttpGet]
		[Authorize]
		public async Task<ActionResult> Summary(int transferId, string returnUrl)
		{
			var transfer = await TransferReader.GetTransfer(User.Identity.GetUserId(), transferId);
			if (transfer == null)
			{
				ViewBag.Message = String.Format(Resources.UserWallet.transferNotFoundError, transferId);
				return View("Error");
			}

			transfer.ReturnUrl = GetLocalReturnUrl(returnUrl);
			return View(transfer);
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> CheckUser(string userName)
		{
			var checkUser = await UserReader.GetUserByName(userName);
			if (checkUser != null)
				return JsonSuccess();

			return JsonError();
		}

		private async Task<TransferCurrencyModel> CreateTransferModel(Cryptopia.Entity.ApplicationUser user, TransferCurrencyModel model)
		{
			var currencyData = await CurrencyReader.GetCurrency(model.Symbol);
			var balanceData = await BalanceReader.GetCurrencyBalance(User.Identity.GetUserId(), currencyData.CurrencyId);
			var verificationData = await UserVerificationReader.GetVerificationStatus(User.Identity.GetUserId());
			var estimatedCoinNzd = await BalanceEstimationService.GetNZDPerCoin(currencyData.CurrencyId);
			model = user.GetTwoFactorModel(TwoFactorComponent.Transfer, model);
			model.Name = currencyData.Name;
			model.CurrencyId = currencyData.CurrencyId;
			model.Symbol = balanceData.Symbol;
			model.Balance = balanceData.Available;
			model.HasWithdrawLimit = verificationData.Limit > 0;
			model.WithdrawLimit = verificationData.Limit;
			model.WithdrawTotal = verificationData.Current;
			model.EstimatedCoinNZD = estimatedCoinNzd;
			return model;
		}
	}
}