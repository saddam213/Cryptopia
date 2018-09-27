using System.Threading.Tasks;
using System.Web.Mvc;
using Cryptopia.Infrastructure.Common.DataTables;
using Cryptopia.Common.User;
using Cryptopia.Common.Utilities;
using Cryptopia.Entity;
using Cryptopia.Enums;
using Microsoft.AspNet.Identity;
using Web.Site.Api.Authentication;
using Web.Site.Helpers;
using Web.Site.Models;
using Cryptopia.Common.Address;
using Cryptopia.Common.Currency;
using System;
using Cryptopia.Base;

namespace Web.Site.Controllers
{
	public class UserSecurityController : BaseUserController
	{
		public IUserSecurityWriter UserSecurityWriter { get; set; }
		public IEncryptionService EncryptionService { get; set; }
		public IUserSyncService UserSyncService { get; set; }
		public IAddressBookWriter AddressBookWriter { get; set; }
		public IAddressBookReader AddressBookReader { get; set; }
		public ICurrencyReader CurrencyReader { get; set; }

		[HttpGet]
		[Authorize]
		public async Task<ActionResult> GetSecurity()
		{
			var user = await UserManager.FindValidByIdAsync(User.Identity.GetUserId());
			if (user == null)
				return Unauthorized();

			var twofactor = await UserManager.GetUserTwoFactorAsync(user.Id, TwoFactorComponent.Settings);
			if (twofactor == null || twofactor.Type == TwoFactorType.None)
			{
				var withdrawTwofactor = await UserManager.GetUserTwoFactorAsync(User.Identity.GetUserId(), TwoFactorComponent.Withdraw);
				return PartialView("_Security", new UserSecurityModel
				{
					ApiModel = new UpdateApiModel
					{
						IsApiEnabled = user.IsApiEnabled,
						IsApiWithdrawEnabled = user.IsApiWithdrawEnabled,
						IsApiUnsafeWithdrawEnabled = user.IsApiUnsafeWithdrawEnabled,
						ApiKey = user.ApiKey,
						ApiSecret = user.ApiSecret
					},
					WithdrawModel = new UpdateWithdrawModel
					{
						AddressBookOnly = !user.IsUnsafeWithdrawEnabled,
						DisableConfirmation = user.DisableWithdrawEmailConfirmation,
						HasWithdrawTfa = withdrawTwofactor != null && withdrawTwofactor.Type != TwoFactorType.None
					}
				});
			}

			var model = new UserUnlockSecurityModel { Type = twofactor.Type };
			if (twofactor.Type == TwoFactorType.Question)
			{
				model.Question1 = twofactor.Data;
				model.Question2 = twofactor.Data3;
			}
			if (twofactor.Type == TwoFactorType.EmailCode)
			{
				var twofactorCode = await UserManager.GenerateUserTwoFactorCodeAsync(TwoFactorType.EmailCode, user.Id);
				if (!await SendTwoFactorCode(user, twofactorCode, twofactor.Data))
				{
					return ViewMessage(new ViewMessageModel(ViewMessageType.Warning, 
						Resources.User.securityUnlockEmailCodeNotSentErrorTitle,
						String.Format(Resources.User.securityUnlockEmailCodeNotSentError, 
							String.Format("<a href='/Support'>{0}</a>", Cryptopia.Resources.General.CryptopiaSupportLink)))
					);
				}
			}

			return PartialView("_UnlockSecurity", model);
		}

		private async Task<bool> SendTwoFactorCode(ApplicationUser user, string token, string email)
		{
			if (user == null)
				return false;

			return await SendEmailAsync(EmailTemplateType.TwoFactorSettingsUnlock, null, email, user.Id, user.UserName, token);
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> UnlockSecurity(UserUnlockSecurityModel model)
		{
			var user = await UserManager.FindValidByIdAsync(User.Identity.GetUserId());
			if (user == null)
				return Unauthorized();

			if (!await UserManager.VerifyUserTwoFactorCodeAsync(TwoFactorComponent.Settings, user.Id, model.Data, model.Data2))
			{
				ModelState.AddModelError("Data", Resources.User.securityUnlockInvalidCodeError);
				return PartialView("_UnlockSecurity", model);
			}

			user.SettingsUnlocked = DateTime.UtcNow.AddMinutes(10);
			await UserManager.UpdateAsync(user);

			var withdrawTwofactor = await UserManager.GetUserTwoFactorAsync(User.Identity.GetUserId(), TwoFactorComponent.Withdraw);
			return PartialView("_Security", new UserSecurityModel
			{
				ApiModel = new UpdateApiModel
				{
					IsApiEnabled = user.IsApiEnabled,
					IsApiWithdrawEnabled = user.IsApiWithdrawEnabled,
					IsApiUnsafeWithdrawEnabled = user.IsApiUnsafeWithdrawEnabled,
					ApiKey = user.ApiKey,
					ApiSecret = user.ApiSecret
				},
				WithdrawModel = new UpdateWithdrawModel
				{
					AddressBookOnly = !user.IsUnsafeWithdrawEnabled,
					DisableConfirmation = user.DisableWithdrawEmailConfirmation,
					HasWithdrawTfa = withdrawTwofactor != null && withdrawTwofactor.Type != TwoFactorType.None
				}
			});
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> UpdatePassword(UpdatePasswordModel model)
		{
			if (!ModelState.IsValid)
			{
				return PartialView("_UpdatePassword", new UpdatePasswordModel());
			}

			if (!await IsSettingsUnlocked())
			{
				ModelState.AddModelError("Error", Resources.User.securityUnlockTokenExpiredError);
				return PartialView("_UpdatePassword", new UpdatePasswordModel());
			}

			var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
			if (!result.Succeeded)
			{
				foreach (var error in result.Errors)
					ModelState.AddModelError("Error", error);

				return PartialView("_UpdatePassword", new UpdatePasswordModel());
			}

			await UserManager.UpdateSecurityStampAsync(User.Identity.GetUserId());
			ModelState.AddModelError("Success", Resources.User.securityPasswordUpdatedMessage);
			return PartialView("_UpdatePassword", new UpdatePasswordModel());
		}

		#region Api Settings

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> UpdateApiSettings(UpdateApiModel model)
		{
			if (!ModelState.IsValid)
			{
				return PartialView("_UpdateApi", model);
			}

			if (!await IsSettingsUnlocked())
			{
				ModelState.AddModelError("Error", Resources.User.securityUnlockTokenExpiredError);
				return PartialView("_UpdateApi", model);
			}

			var result = await UserSecurityWriter.UpdateApiSettings(User.Identity.GetUserId(), model);
			if (!ModelState.IsWriterResultValid(result))
				return PartialView("_UpdateApi", model);

			await UserSyncService.SyncUser(User.Identity.GetUserId());
			await ApiKeyStore.UpdateApiAuthKey(model);
			return PartialView("_UpdateApi", model);
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public ActionResult GenerateApiKey()
		{
			var result = EncryptionService.GenerateEncryptionKeyPair();
			if (!result.Success || !result.HasResult)
				return JsonError(Resources.User.securityApiGenerationFailedMessage);

			return Json(new
			{
				Success = true,
				Key = result.Result.PublicKey,
				Secret = result.Result.PrivateKey
			});
		}

		#endregion

		#region AddressBook

		[HttpGet]
		[Authorize]
		public async Task<ActionResult> GetAddressBook(DataTablesModel param)
		{
			return DataTable(await AddressBookReader.GetAddressBookDataTable(User.Identity.GetUserId(), param));
		}

		[HttpGet]
		[Authorize]
		public async Task<ActionResult> CreateAddressBook()
		{
			return View("CreateAddressBookModal", new AddressBookModel
			{
				Currencies = await CurrencyReader.GetCurrencies()
			});
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> CreateAddressBook(AddressBookModel model)
		{
			if (!await IsSettingsUnlocked())
				return UnauthorizedModal();

			if (!ModelState.IsValid)
			{
				model.Currencies = await CurrencyReader.GetCurrencies();
				return View("CreateAddressBookModal", model);
			}

			var currency = await CurrencyReader.GetCurrency(model.CurrencyId);
			if (currency == null)
			{
				model.Currencies = await CurrencyReader.GetCurrencies();
				return View("CreateAddressBookModal", model);
			}
	
			model.CurrencyType = currency.Type;
			model.AddressType = currency.AddressType;
			var result = await AddressBookWriter.CreateAddressBook(User.Identity.GetUserId(), model);
			if (!ModelState.IsWriterResultValid(result))
			{
				model.Currencies = await CurrencyReader.GetCurrencies();
				return View("CreateAddressBookModal", model);
			}
			return CloseModalSuccess();
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> DeleteAddressBook(int addressBookId)
		{
			if (!await IsSettingsUnlocked())
				return JsonError();

			var result = await AddressBookWriter.DeleteAddressBook(User.Identity.GetUserId(), addressBookId);
			if (!ModelState.IsWriterResultValid(result))
				return JsonError();

			return JsonSuccess();
		}

		#endregion

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> UpdateWithdrawSettings(UpdateWithdrawModel model)
		{
			if (!await IsSettingsUnlocked())
				return JsonError();

			if (model.DisableConfirmation)
			{
				var twofactor = await UserManager.GetUserTwoFactorAsync(User.Identity.GetUserId(), TwoFactorComponent.Withdraw);
				if (twofactor == null || twofactor.Type == TwoFactorType.None)
					return JsonError(Resources.User.securityTfaEmailErrorMessage);
			}

			var result = await UserSecurityWriter.UpdateWithdrawSettings(User.Identity.GetUserId(), model);
			if (!result.Success)
				return Json(result);

			await UserSyncService.SyncUser(User.Identity.GetUserId());
			return Json(result);
		}

		private async Task<bool> IsSettingsUnlocked()
		{
			var user = await UserManager.FindValidByIdAsync(User.Identity.GetUserId());
			if (user == null)
				return false;

			var twofactor = await UserManager.GetUserTwoFactorAsync(user.Id, TwoFactorComponent.Settings);
			if (twofactor == null || twofactor.Type == TwoFactorType.None)
				return true;

			return user.SettingsUnlocked > DateTime.UtcNow;
		}
	}
}