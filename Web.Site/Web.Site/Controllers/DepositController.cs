using Cryptopia.Common.Address;
using Cryptopia.Common.Currency;
using Cryptopia.Common.Deposit;
using Cryptopia.Common.UserVerification;
using Cryptopia.Common.Utilities;
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
	public class DepositController : BaseUserController
	{
		public ICurrencyReader CurrencyReader { get; set; }
		public IAddressReader AddressReader { get; set; }
		public IUserVerificationReader UserVerificationReader { get; set; }

		[HttpGet]
		[Authorize]
		public async Task<ActionResult> Index(string returnUrl)
		{
			var currencyData = await CurrencyReader.GetCurrencies();
			return View(new DepositViewModel
			{
				Currencies = currencyData,
				ReturnUrl = GetLocalReturnUrl(returnUrl)
			});
		}

		[HttpGet]
		[Authorize]
		public async Task<ActionResult> Display(string currency, string returnUrl)
		{
			var user = await UserManager.FindValidByIdAsync(User.Identity.GetUserId());
			var currencyData = await CurrencyReader.GetCurrency(currency);
			if (currencyData == null || user == null)
			{
				ViewBag.Message = String.Format(Resources.UserWallet.depositCurrencyNotFoundError, currency);
				return View("Error");
			}

			if (currencyData.CurrencyId == Constant.NZDT_ID && !UserVerificationReader.IsVerified(user.VerificationLevel))
			{
				return View("NotVerified");
			}

			var addressData = await AddressReader.GetDisplayAddress(User.Identity.GetUserId(), currency);
			addressData.ReturnUrl = GetLocalReturnUrl(returnUrl);
			return View("Display", addressData);
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> GenerateAddress(int currencyId)
		{
			var addressData = await AddressReader.GetAddress(User.Identity.GetUserId(), currencyId);
			return Json(new
			{
				Success = string.IsNullOrEmpty(addressData?.ErrorMessage),
				AddressData = addressData?.AddressData,
				AddressData2 = addressData?.AddressData2,
				AddressData3 = addressData?.AddressData3,
				Message = addressData?.ErrorMessage,
				QrCode = addressData?.QrCode
			});
		}
	}
}