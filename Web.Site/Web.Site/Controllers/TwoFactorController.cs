using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Cryptopia.Common.TwoFactor;
using Cryptopia.Entity;
using Cryptopia.Enums;
using Microsoft.AspNet.Identity;
using Web.Site.Helpers;
using Web.Site.Models;
using Cryptopia.Common.User;
using Cryptopia.Data.DataContext;
using System.Data.Entity;
using System;
using Cryptopia.Infrastructure.Helpers;
using System.ComponentModel.DataAnnotations;

namespace Web.Site.Controllers
{
	public class TwoFactorController : BaseUserController
	{
		public IUserSyncService UserSyncService { get; set; }

		[Authorize]
		[ChildActionOnly]
		public ActionResult GetTwoFactor(TwoFactorComponent component)
		{
			var user = UserManager.FindById(User.Identity.GetUserId());
			if (user == null)
				return Unauthorized();

			var twoFactor = user.TwoFactor.FirstOrDefault(x => x.Component == component)
							?? new UserTwoFactor { Type = TwoFactorType.None };
			return PartialView("_ViewPartial", new ViewTwoFactorModel
			{
				Type = twoFactor.Type,
				ComponentType = component
			});
		}

		#region Create

		[HttpGet]
		[Authorize]
		public async Task<ActionResult> Create(TwoFactorComponent componentType)
		{
			var user = UserManager.FindById(User.Identity.GetUserId());
			if (user == null)
				return Unauthorized();

			// If twofactor exists something is dodgy, return unauthorised
			var twofactor = user.TwoFactor.FirstOrDefault(x => x.Component == componentType && x.Type != TwoFactorType.None);
			if (twofactor != null)
				return RedirectToRoute("Security");

			var hasExistingGoogle = user.TwoFactor.Any(x => x.Type == TwoFactorType.GoogleCode);
			var hasExistingCryptopia = user.TwoFactor.Any(x => x.Type == TwoFactorType.CryptopiaCode);

			return await Task.FromResult(View(new CreateTwoFactorModel
			{
				ComponentType = componentType,
				GoogleData = GoogleAuthenticationHelper.GetGoogleTwoFactorData(user.UserName),
				HasExistingGoogle = hasExistingGoogle,
				HasExistingCryptopia = hasExistingCryptopia,
				ApplyToAllEmpty = true
			}));
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Create(CreateTwoFactorModel model)
		{
			await Validate(ModelState, model);
			if (!ModelState.IsValid)
				return View("Create", model);

			var user = UserManager.FindById(User.Identity.GetUserId());
			if (user == null)
				return Unauthorized();

			// If twofactor exists something is dodgy, return unauthorised
			var userTwoFactor = user.TwoFactor.ToList();
			var twofactor = userTwoFactor.FirstOrDefault(x => x.Component == model.ComponentType);
			if (twofactor != null && twofactor.Type != TwoFactorType.None)
				return RedirectToRoute("Security");

			if (model.Type == TwoFactorType.GoogleCode)
			{
				var existing = user.TwoFactor.FirstOrDefault(x => x.Type == TwoFactorType.GoogleCode);
				if (existing != null)
				{
					model.GoogleData.PrivateKey = existing.Data;
					model.GoogleData.PublicKey = existing.Data2;
				}
			}

			if (model.Type == TwoFactorType.CryptopiaCode)
			{
				var existing = user.TwoFactor.FirstOrDefault(x => x.Type == TwoFactorType.CryptopiaCode);
				if (existing == null)
				{
					using (var context = new ApplicationDbContext())
					{
						if (!await context.TwoFactorCode.AnyAsync(x => x.UserId == user.Id && x.SerialNumber == model.CryptopiaSerial))
						{
							ModelState.AddModelError("", Resources.Authorization.twoFactorCryptopiaNoDeviceError);
							return View("Create", model);
						}
					}
				}
			}

			if (model.ApplyToAllEmpty)
			{
				foreach (TwoFactorComponent twoFactorComponent in Enum.GetValues(typeof(TwoFactorComponent)))
				{
					var existing = userTwoFactor.FirstOrDefault(x => x.Component == twoFactorComponent);
					if (existing != null && existing.Type != TwoFactorType.None)
						continue;

					if (existing == null)
					{
						existing = new UserTwoFactor();
						SetTwoFactorValues(twoFactorComponent, model, existing);
						user.TwoFactor.Add(existing);
						continue;
					}
					SetTwoFactorValues(twoFactorComponent, model, existing);
				}
				await UserManager.UpdateAsync(user);
				return RedirectToRoute("Security");
			}

			// If no TFA exists, create and redirect to TFA view partial
			if (twofactor == null)
			{
				twofactor = new UserTwoFactor();
				SetTwoFactorValues(model.ComponentType, model, twofactor);
				user.TwoFactor.Add(twofactor);
				await UserManager.UpdateAsync(user);
				return RedirectToRoute("Security");
			}

			SetTwoFactorValues(twofactor.Component, model, twofactor);
			await UserManager.UpdateAsync(user);
			return RedirectToRoute("Security");
		}

		#endregion

		#region Remove

		[HttpGet]
		[Authorize]
		public async Task<ActionResult> Remove(TwoFactorComponent componentType)
		{
			var user = UserManager.FindById(User.Identity.GetUserId());
			if (user == null)
				return Unauthorized();

			var twofactor = user.TwoFactor.FirstOrDefault(x => x.Component == componentType) ??
											new UserTwoFactor { Type = TwoFactorType.None };
			if (twofactor.Type == TwoFactorType.EmailCode)
			{
				var twofactorCode = await UserManager.GenerateUserTwoFactorCodeAsync(TwoFactorType.EmailCode, user.Id);
				if (!await SendTwoFactorCode(user, twofactorCode, componentType, twofactor.Data))
				{
					return ViewMessage(new ViewMessageModel(ViewMessageType.Warning,
						Resources.Authorization.twoFactorRemoveEmailSendFaildTitle,
						String.Format(Resources.Authorization.twoFactorRemoveEmailSendFaildMessage,
							String.Format("<a href='/Support'>{0}</a>", Cryptopia.Resources.General.CryptopiaSupportLink)))
					);
				}
			}

			var model = new RemoveTwoFactorModel
			{
				Type = twofactor.Type,
				ComponentType = componentType,
			};

			if (twofactor.Type == TwoFactorType.Question)
			{
				model.Question1 = twofactor.Data;
				model.Question2 = twofactor.Data3;
			}

			return View("Remove", model);
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Remove(RemoveTwoFactorModel model)
		{
			if (!ModelState.IsValid)
				return View("Remove", model);

			var user = UserManager.FindById(User.Identity.GetUserId());
			if (user == null)
				return Unauthorized();

			var twofactor = user.TwoFactor.FirstOrDefault(x => x.Component == model.ComponentType && x.Type == model.Type);
			if (twofactor == null)
				return RedirectToRoute("Security");

			if (!await UserManager.VerifyUserTwoFactorCodeAsync(model.ComponentType, user.Id, model.Data, model.Data2))
			{
				// failed to validate last TFA
				ModelState.AddModelError("", Resources.Authorization.twoFactorRemoveIncorrectTfaErrorMessage);
				return View("Remove", model);
			}

			// Delete TFA
			twofactor.ClearData();
			twofactor.Type = TwoFactorType.None;
			//twofactor.Updated = DateTime.UtcNow;

			if (model.ComponentType == TwoFactorComponent.Withdraw)
				user.DisableWithdrawEmailConfirmation = false;

			await UserManager.UpdateAsync(user);
			await UserSyncService.SyncUser(user.Id);
			return RedirectToRoute("Security");
		}

		#endregion

		#region Helpers

		[HttpPost]
		[Authorize]
		public async Task<ActionResult> SendEmailCode(TwoFactorComponent componentType, string dataEmail)
		{
			var user = UserManager.FindById(User.Identity.GetUserId());
			if (user == null)
				return JsonError("Unauthorized");

			if (!ValidationHelpers.IsValidEmailAddress(dataEmail))
				return JsonError(string.Format(Resources.Authorization.twoFactorInvalidEmailError, dataEmail));

			var emailSent = false;
			var twofactorCode =
				await UserManager.GenerateUserTwoFactorCodeAsync(TwoFactorType.EmailCode, User.Identity.GetUserId());
			if (componentType == TwoFactorComponent.Settings)
				emailSent =
					await SendEmailAsync(EmailTemplateType.TwoFactorSettingsUnlock, null, dataEmail, user.Id, user.UserName, twofactorCode);

			if (componentType == TwoFactorComponent.Withdraw)
				emailSent =
					await SendEmailAsync(EmailTemplateType.TwoFactorWithdraw, null, dataEmail, user.Id, user.UserName, twofactorCode);

			if (componentType == TwoFactorComponent.Transfer)
				emailSent =
					await SendEmailAsync(EmailTemplateType.TwoFactorTransfer, null, dataEmail, user.Id, user.UserName, twofactorCode);

			if (componentType == TwoFactorComponent.Tip)
				emailSent = await SendEmailAsync(EmailTemplateType.TwoFactorTip, null, dataEmail, user.Id, user.UserName, twofactorCode);

			if (componentType == TwoFactorComponent.Login)
				emailSent = await SendEmailAsync(EmailTemplateType.TwoFactorLogin, null, dataEmail, user.Id, user.UserName, twofactorCode);

			if (emailSent)
				return JsonSuccess();

			return JsonError();
		}

		[HttpPost]
		[Authorize]
		public async Task<ActionResult> SendUnlockTwoFactorCode(TwoFactorComponent componentType)
		{
			var user = UserManager.FindById(User.Identity.GetUserId());
			if (user == null)
				return JsonError("Unauthorized");

			var twofactor = user.TwoFactor.FirstOrDefault(x => x.Component == componentType && x.Type == TwoFactorType.EmailCode);
			if (twofactor == null)
				return JsonError("Unauthorized");

			var email = twofactor.Data;
			var emailSent = false;
			var twofactorCode =
				await UserManager.GenerateUserTwoFactorCodeAsync(TwoFactorType.EmailCode, User.Identity.GetUserId());
			if (componentType == TwoFactorComponent.Settings)
				emailSent =
					await SendEmailAsync(EmailTemplateType.TwoFactorSettingsUnlock, null, email, user.Id, user.UserName, twofactorCode);

			if (componentType == TwoFactorComponent.Withdraw)
				emailSent =
					await SendEmailAsync(EmailTemplateType.TwoFactorWithdraw, null, email, user.Id, user.UserName, twofactorCode);

			if (componentType == TwoFactorComponent.Transfer)
				emailSent =
					await SendEmailAsync(EmailTemplateType.TwoFactorTransfer, null, email, user.Id, user.UserName, twofactorCode);

			if (componentType == TwoFactorComponent.Tip)
				emailSent = await SendEmailAsync(EmailTemplateType.TwoFactorTip, null, email, user.Id, user.UserName, twofactorCode);

			if (componentType == TwoFactorComponent.Login)
				emailSent =
					await SendEmailAsync(EmailTemplateType.TwoFactorLogin, null, email, user.Id, user.UserName, twofactorCode);

			if (emailSent)
				return JsonSuccess();

			return JsonError();
		}

		[HttpPost]
		[Authorize]
		public async Task<ActionResult> VerifyEmailCode(TwoFactorComponent componentType, string code)
		{
			var user = UserManager.FindById(User.Identity.GetUserId());
			if (user == null)
				return JsonError("Unauthorized");

			if (await UserManager.VerifyTwoFactorTokenAsync(user.Id, TwoFactorType.EmailCode.ToString(), code))
			{
				return JsonSuccess();
			}
			return JsonError();
		}

		[HttpPost]
		[Authorize]
		public ActionResult VerifyGoogleCode(string key, string code)
		{
			return GoogleAuthenticationHelper.VerifyGoogleTwoFactorCode(key, code) ? JsonSuccess() : JsonError();
		}

		[HttpPost]
		[Authorize]
		public async Task<ActionResult> VerifyCryptopiaCode(string serialNumber, string code)
		{
			var userId = User.Identity.GetUserId();
			using (var context = new ApplicationDbContext())
			{
				if (!await context.TwoFactorCode.AnyAsync(x => x.UserId == userId && x.SerialNumber == serialNumber))
				{
					return JsonError(Resources.Authorization.twoFactorCryptopiaInvalidSerialError);
				}
			}

			if (await CryptopiaAuthenticationHelper.VerifyTwoFactorCode(User.Identity.GetUserId(), code))
			{
				return JsonSuccess(Resources.Authorization.twoFactorCryptopiaActivatedMessage);
			}
			return JsonError(Resources.Authorization.twoFactorCryptopiaInvaidCodeError);
		}

		private async Task<bool> SendTwoFactorCode(ApplicationUser user, string token, TwoFactorComponent type, string email)
		{
			if (user == null)
				return false;

			if (type == TwoFactorComponent.Settings)
				return await SendEmailAsync(EmailTemplateType.TwoFactorSettingsUnlock, null, email, user.Id, user.UserName, token);

			if (type == TwoFactorComponent.Withdraw)
				return await SendEmailAsync(EmailTemplateType.TwoFactorWithdraw, null, email, user.Id, user.UserName, token);

			if (type == TwoFactorComponent.Transfer)
				return await SendEmailAsync(EmailTemplateType.TwoFactorTransfer, null, email, user.Id, user.UserName, token);

			if (type == TwoFactorComponent.Tip)
				return await SendEmailAsync(EmailTemplateType.TwoFactorTip, null, email, user.Id, user.UserName, token);

			if (type == TwoFactorComponent.Login)
				return await SendEmailAsync(EmailTemplateType.TwoFactorLogin, null, email, user.Id, user.UserName, token);

			return false;
		}

		private void SetTwoFactorValues(TwoFactorComponent componentType, CreateTwoFactorModel model, UserTwoFactor entity)
		{
			entity.ClearData();
			entity.Type = model.Type;
			entity.Component = componentType;
			entity.IsEnabled = true;
			if (model.Type == TwoFactorType.EmailCode)
			{
				entity.Data = model.DataEmail;
			}
			else if (model.Type == TwoFactorType.PinCode)
			{
				entity.Data = model.DataPin;
			}
			else if (model.Type == TwoFactorType.GoogleCode)
			{
				entity.Data = model.GoogleData.PrivateKey;
				entity.Data2 = model.GoogleData.PublicKey;
			}
			else if (model.Type == TwoFactorType.Question)
			{
				entity.Data = model.DataQuestion1;
				entity.Data2 = model.DataAnswer1;
				entity.Data3 = model.DataQuestion2;
				entity.Data4 = model.DataAnswer2;
			}
		}

		private async Task Validate(ModelStateDictionary modelstate, CreateTwoFactorModel model)
		{
			if (model.Type == TwoFactorType.EmailCode)
			{
				if (string.IsNullOrEmpty(model.DataEmail))
					modelstate.AddModelError("DataEmail", Resources.Authorization.twoFactorCreateEmailRequiredError);

				if (!ValidationHelpers.IsValidEmailAddress(model.DataEmail))
					modelstate.AddModelError("DataEmail", Resources.Authorization.twoFactorCreateEmailInvalidError);
			}
			else if (model.Type == TwoFactorType.GoogleCode)
			{
				if (!model.GoogleData.IsValid)
					modelstate.AddModelError("", Resources.Authorization.twoFactorCreateGoogleApiError);
			}
			else if (model.Type == TwoFactorType.PinCode)
			{
				if (model.DataPin.Length < 4 || model.DataPin.Length > 8)
					modelstate.AddModelError("DataPin", Cryptopia.Resources.Authorization.twoFactorDataPinPatternError);
			}
			else if (model.Type == TwoFactorType.CryptopiaCode)
			{
				using (var context = new ApplicationDbContext())
				{
					var userId = User.Identity.GetUserId();
					var userTwofactor = await context.TwoFactorCode.FirstOrDefaultAsync(x => x.UserId == userId);
					if (userTwofactor == null)
						modelstate.AddModelError("", Resources.Authorization.twoFactorCreateCryptopiaNoDeviceError);
				}
			}
			else if (model.Type == TwoFactorType.Question)
			{
				if (string.IsNullOrEmpty(model.DataQuestion1))
					modelstate.AddModelError("DataQuestion1", Resources.Authorization.twoFactorCreateQuestionRequiredError);
				if (string.IsNullOrEmpty(model.DataQuestion2))
					modelstate.AddModelError("DataQuestion2", Resources.Authorization.twoFactorCreateQuestionRequiredError);
				if (string.IsNullOrEmpty(model.DataAnswer1))
					modelstate.AddModelError("DataAnswer1", Resources.Authorization.twoFactorCreateAnswerRequiredError);
				if (string.IsNullOrEmpty(model.DataAnswer2))
					modelstate.AddModelError("DataAnswer2", Resources.Authorization.twoFactorCreateAnswerRequiredError);
			}
		}

		#endregion
	}
}