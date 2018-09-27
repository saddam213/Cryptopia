using Cryptopia.Base;
using Cryptopia.Common.Address;
using Cryptopia.Common.Balance;
using Cryptopia.Common.Currency;
using Cryptopia.Common.Deposit;
using Cryptopia.Common.Trade;
using Cryptopia.Common.UserVerification;
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
	public class WithdrawController : BaseUserController
	{
		public ICurrencyReader CurrencyReader { get; set; }
		public IBalanceReader BalanceReader { get; set; }
		public IAddressBookReader AddressBookReader { get; set; }
		public IWithdrawWriter WithdrawWriter { get; set; }
		public IWithdrawReader WithdrawReader { get; set; }
		public IUserVerificationReader UserVerificationReader { get; set; }
		public IBalanceEstimationService BalanceEstimationService { get; set; }


		[HttpGet]
		[Authorize]
		public async Task<ActionResult> Index(string returnUrl)
		{
			var currencyData = await CurrencyReader.GetCurrencies();
			if (currencyData == null)
			{
				ViewBag.Message = "An unknown error occurred, if problems persit please contact Cryptopia support.";
				return View("Error");
			}

			return View(new WithdrawViewModel
			{
				Currencies = currencyData,
				ReturnUrl = GetLocalReturnUrl(returnUrl)
			});
		}

		[HttpGet]
		[Authorize]
		public async Task<ActionResult> Create(string currency, string returnUrl)
		{
			var user = await UserManager.FindValidByIdAsync(User.Identity.GetUserId());
			var currencyData = await CurrencyReader.GetCurrency(currency);
			if (currencyData == null || user == null)
			{
				ViewBag.Message = $"Currency {currency} not found.";
				return View("Error");
			}
		
			if (currencyData.Status == CurrencyStatus.Maintenance || currencyData.Status == CurrencyStatus.Offline)
			{
				ViewBag.HideSupportLink = true;
				ViewBag.SubTitle = $"Status: {currencyData.Status}";
				ViewBag.Message = $"Unfortunately we are unable to process your withdrawal at this time.<br /><br />Status: {currencyData.Status}<br />Reason: {(string.IsNullOrEmpty(currencyData.StatusMessage) ? "Unknown" : currencyData.StatusMessage)}";
				return View("Error");
			}

			if(currencyData.CurrencyId == Constant.NZDT_ID && !UserVerificationReader.IsVerified(user.VerificationLevel))
			{
				return View("NotVerified");
			}

			return View(await CreateWithdrawModel(user, new WithdrawCurrencyModel
			{
				Name = currencyData.Name,
				Symbol = currencyData.Symbol,
				ReturnUrl = GetLocalReturnUrl(returnUrl)
			}));
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Create(WithdrawCurrencyModel model)
		{
			var currencyData = await CurrencyReader.GetCurrency(model.Symbol);
			var user = await UserManager.FindValidByIdAsync(User.Identity.GetUserId());
			if (currencyData == null || user == null)
			{
				ViewBag.Message = $"Currency {model.Symbol} not found.";
				return View("Error");
			}

			if (currencyData.CurrencyId == Constant.NZDT_ID && !UserVerificationReader.IsVerified(user.VerificationLevel))
			{
				return View("NotVerified");
			}

			// Verify Address
			var address = model.AddressData?.Trim();
			if (string.IsNullOrEmpty(address))
			{
				address = model.SelectedAddress;
			}
			else if (currencyData.AddressType != AddressType.Standard)
			{
				if (currencyData.AddressType == AddressType.PaymentId && !CurrencyExtensions.IsValidPaymentId(model.AddressData2?.Trim()))
				{
					ModelState.AddModelError("AddressData2", $"Please enter a valid Cryptonote PaymentId.");
					return View(await CreateWithdrawModel(user, model));
				}
				address = $"{model.AddressData?.Trim()}:{model.AddressData2?.Trim()}";
				if (currencyData.Type == CurrencyType.Fiat)
					address = address.TrimEnd(':');
			}
			if (string.IsNullOrEmpty(address))
			{
				ModelState.AddModelError(user.IsUnsafeWithdrawEnabled ? "AddressData" : "AddressBookError", $"Please enter a valid {model.Symbol} address");
				return View(await CreateWithdrawModel(user, model));
			}

			// Verify Amount
			var withdrawAmount = decimal.Round(model.Amount, currencyData.CurrencyDecimals);
			if (withdrawAmount < currencyData.WithdrawMin || withdrawAmount >= currencyData.WithdrawMax)
			{
				ModelState.AddModelError("Amount", $"Withdrawal amount must be between {currencyData.WithdrawMin} {currencyData.Symbol} and {currencyData.WithdrawMax} {currencyData.Symbol}");
				return View(await CreateWithdrawModel(user, model));
			}

			// Verify Two Factor
			if (!await UserManager.VerifyUserTwoFactorCodeAsync(TwoFactorComponent.Withdraw, user.Id, model.Code1, model.Code2))
			{
				ModelState.AddModelError("TwoFactorError", "The TFA code entered was incorrect or has expired, please try again.");
				return View(await CreateWithdrawModel(user, model));
			}

			// Create withdraw
		
			var twoFactortoken = await GenerateUserTwoFactorTokenAsync(TwoFactorTokenType.WithdrawConfirm, user.Id);
			var result = await WithdrawWriter.CreateWithdraw(User.Identity.GetUserId(), new CreateWithdrawModel
			{
				Address = address,
				Amount = withdrawAmount,
				CurrencyId = model.CurrencyId,
				TwoFactorToken = twoFactortoken,
				Type = WithdrawType.Normal,
			});
			if (!result.Success)
			{
				ModelState.AddModelError("Error", result.Message);
				return View(await CreateWithdrawModel(user, model));
			}

			// Send Confirmation email
			if (!user.DisableWithdrawEmailConfirmation)
			{
				await SendConfirmationEmail(user, model.Symbol, withdrawAmount - model.Fee, address, result.Result, twoFactortoken, currencyData.AddressType);
			}

			return RedirectToAction("Summary", new { withdrawId = result.Result, returnUrl = GetLocalReturnUrl(model.ReturnUrl) });
		}

		[HttpGet]
		[Authorize]
		public async Task<ActionResult> Summary(int withdrawId, string returnUrl)
		{
			var withdraw = await WithdrawReader.GetWithdrawal(User.Identity.GetUserId(), withdrawId);
			if (withdraw == null)
			{
				ViewBag.Message = $"Withdrawal #{withdrawId} not found.";
				return View("Error");
			}

			withdraw.ReturnUrl = GetLocalReturnUrl(returnUrl);
			return View(withdraw);
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> CancelWithdraw(int withdrawid)
		{
			var result = await WithdrawWriter.CancelWithdraw(User.Identity.GetUserId(), new CancelWithdrawModel
			{
				WithdrawId = withdrawid
			});

			if (!result.Success)
				return JsonError(result.Message);

			return JsonSuccess(string.Format("Withdrawal #{0} has been successfully canceled.", withdrawid));
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ResendWithdrawConfirmation(int withdrawid)
		{
			var user = await UserManager.FindValidByIdAsync(User.Identity.GetUserId());
			if (user == null)
				return JsonError("Unauthorized");

			var twoFactortoken = await GenerateUserTwoFactorTokenAsync(TwoFactorTokenType.WithdrawConfirm, user.Id);
			var model = new UpdateTwoFactorTokenModel
			{
				WithdrawId = withdrawid,
				TwoFactorToken = twoFactortoken
			};

			var response = await WithdrawWriter.UpdateTwoFactorToken(User.Identity.GetUserId(), model);
			if (!response.Success)
				return JsonError(response.Message);

			if (!await SendConfirmationEmail(user, model.Symbol, model.Amount, model.Address, withdrawid, twoFactortoken, model.AddressType))
				return JsonError("Failed to send email, if problem persists please contact Cryptopia support.");

			return JsonSuccess($"Successfully sent. Please check {user.Email} for a secure link to confirm this withdrawal.");
		}

		[HttpGet]
		[AllowAnonymous]
		public async Task<ActionResult> WithdrawConfirm(string userId, string code, int withdrawid)
		{
			if (!await VerifyUserTwoFactorTokenAsync(TwoFactorTokenType.WithdrawConfirm, userId, code))
			{
				ViewBag.SubTitle = "Invalid Security Token";
				ViewBag.Message = $"Security token for withdraw #{withdrawid} is invalid or has expired, You can send a new one from the <a href='/WithdrawHistory'>Withdraw History</a> section in your account page.";
				return View("Error");
			}

			var result = await WithdrawWriter.ConfirmWithdraw(userId, new ConfirmWithdrawModel
			{
				WithdrawId = withdrawid,
				TwoFactorToken = code
			});

			if (!result.Success)
			{
				ViewBag.SubTitle = "Invalid Security Token";
				ViewBag.Message = $"Security token for withdraw #{withdrawid} is invalid or has expired, You can send a new one from the <a href='/WithdrawHistory'>Withdraw History</a> section in your account page.";
				return View("Error");
			}

			ViewBag.SubTitle = "Withdrawal Confirmed";
			ViewBag.Message = $"Your withdrawal has been confirmed and will be processed in a few minutes.";
			return View("Success");
		}

		[HttpGet]
		[AllowAnonymous]
		public async Task<ActionResult> WithdrawCancel(string userid, string code, int withdrawid)
		{
			if (!await VerifyUserTwoFactorTokenAsync(TwoFactorTokenType.WithdrawCancel, userid, code))
			{
				ViewBag.SubTitle = "Invalid Security Token";
				ViewBag.Message = $"Security token for cancellation of withdraw #{withdrawid} is invalid or has expired.";
				return View("Error");
			}

			var result = await WithdrawWriter.CancelWithdraw(User.Identity.GetUserId(), new CancelWithdrawModel
			{
				WithdrawId = withdrawid
			});

			if (!result.Success)
			{
				ViewBag.SubTitle = "Withdraw Cancel Failed.";
				ViewBag.Message = result.Message;
				return View("Error");
			}

			ViewBag.SubTitle = "Withdrawal Canceled";
			ViewBag.Message = $"Withdrawal #{withdrawid} has been successfully canceled.";
			return View("Success");
		}

		private async Task<WithdrawCurrencyModel> CreateWithdrawModel(Cryptopia.Entity.ApplicationUser user, WithdrawCurrencyModel model)
		{
			var currencyData = await CurrencyReader.GetCurrency(model.Symbol);
			var balanceData = await BalanceReader.GetCurrencyBalance(User.Identity.GetUserId(), currencyData.CurrencyId);
			var addressBook = await AddressBookReader.GetAddressBook(User.Identity.GetUserId(), currencyData.CurrencyId);
			var verificationData = await UserVerificationReader.GetVerificationStatus(User.Identity.GetUserId());
			var estimatedCoinNzd = await BalanceEstimationService.GetNZDPerCoin(currencyData.CurrencyId);
			model = user.GetTwoFactorModel(TwoFactorComponent.Withdraw, model);
			model.Name = currencyData.Name;
			model.CurrencyId = currencyData.CurrencyId;
			model.CurrencyType = currencyData.Type;
			model.Symbol = balanceData.Symbol;
			model.Balance = balanceData.Available;
			model.Fee = currencyData.WithdrawFee;
			model.WithdrawFeeType = currencyData.WithdrawFeeType;
			model.MinWithdraw = currencyData.WithdrawMin;
			model.MaxWithdraw = currencyData.WithdrawMax;
			model.AddressBookOnly = !user.IsUnsafeWithdrawEnabled;
			model.AddressBook = addressBook;
			model.AddressType = currencyData.AddressType;
			model.HasWithdrawLimit = verificationData.Limit > 0;
			model.WithdrawLimit = verificationData.Limit;
			model.WithdrawTotal = verificationData.Current;
			model.EstimatedCoinNZD = estimatedCoinNzd;
			model.Instructions = currencyData.WithdrawInstructions;
			model.Message = currencyData.WithdrawMessage;
			model.MessageType = currencyData.WithdrawMessageType.ToString().ToLower();
			model.Decimals = currencyData.CurrencyDecimals;
			return model;
		}

		private async Task<bool> SendConfirmationEmail(Cryptopia.Entity.ApplicationUser user, string symbol, decimal amount, string address, int withdrawId, string confirmToken, AddressType addressType)
		{
			var cancelWithdrawToken = await GenerateUserTwoFactorTokenAsync(TwoFactorTokenType.WithdrawCancel, user.Id);
			var confirmlink = Url.Action("WithdrawConfirm", "Withdraw", new { userId = user.Id, code = confirmToken, withdrawid = withdrawId }, protocol: Request.Url.Scheme);
			var cancellink = Url.Action("WithdrawCancel", "Withdraw", new { userId = user.Id, code = cancelWithdrawToken, withdrawid = withdrawId }, protocol: Request.Url.Scheme);
			if(addressType != AddressType.Standard)
			{
				address = address.Replace(":", $"</strong><br />{addressType}: <strong>");
			}
			return await SendEmailAsync(EmailTemplateType.ConfirmWithdraw, withdrawId, user.Email, user.Id, user.UserName, withdrawId, symbol, amount, address, confirmlink, cancellink);
		}
	}
}