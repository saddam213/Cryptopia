using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Web.Site.Models;
using System.Security.Claims;
using Cryptopia.Common.User;
using Cryptopia.Data.DataContext;
using Cryptopia.Entity;
using Cryptopia.Enums;
using Cryptopia.Infrastructure.Helpers;
using hbehr.recaptcha;
using Web.Site.Helpers;

namespace Web.Site.Controllers
{
	public class LoginController : BaseUserController
	{
		public IUserSyncService UserSyncService { get; set; }

		#region Login

		/// <summary>
		/// GET: Login.
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[AllowAnonymous]
		public ActionResult Login(string returnUrl)
		{
			ViewBag.ReturnUrl = returnUrl;
			return View(new LoginViewModel());
		}



		/// <summary>
		/// POST: Login.
		/// </summary>
		/// <param name="model">The model.</param>
		/// <param name="returnUrl">The return URL.</param>
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
		{
			if (!CryptopiaAuthenticationHelper.ValidateCaptcha())
			{
				ModelState.AddModelError("", Resources.Authorization.reCaptchaError);
				return View(model);
			}

			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var user = await UserManager.FindByEmailAsync(model.EmailAddress);
			if (user != null && !user.IsDisabled)
			{
				// is the users email confirmed
				if (!await UserManager.IsEmailConfirmedAsync(user.Id))
				{
					ModelState.AddModelError("", Resources.Authorization.loginConfirmationEmailSentMessage);
					return View(model);
				}

				// is the user locked out
				if (await UserManager.IsLockedOutAsync(user.Id))
				{
					var expires = user.LockoutEndDateUtc.HasValue
						? (user.LockoutEndDateUtc.Value - DateTime.UtcNow).ToReadableString()
						: TimeSpan.FromHours(24).ToReadableString();
					ModelState.AddModelError("", Resources.Authorization.loginAccountIsLockedError);
					ModelState.AddModelError("", Resources.Authorization.loginAccountLockExpiresError + ' ' + expires);
					return View(model);
				}

				// is the password correct
				if (await UserManager.CheckPasswordAsync(user, model.Password))
				{
					await UserSyncService.SyncUser(user.Id);
					await UserManager.ResetAccessFailedCountAsync(user.Id);

					var loginTwoFactor = user.TwoFactor.FirstOrDefault(x => x.Component == TwoFactorComponent.Login);
					if (loginTwoFactor != null && loginTwoFactor.Type != TwoFactorType.None)
					{
						SetTwoFactorLoginAuthCookie(user.Id);
						if (loginTwoFactor.Type == TwoFactorType.EmailCode)
						{
							var code = await GenerateUserTwoFactorCodeAsync(TwoFactorType.EmailCode, user.Id, true);
							await SendEmailAsync(EmailTemplateType.TwoFactorLogin, null, loginTwoFactor.Data, user.Id, user.UserName, code);
						}
						return RedirectToAction("VerifyLoginTwoFactor");
					}

					// No twofactor just sign in
					await SignInAsync(user, false);
					if (string.IsNullOrEmpty(returnUrl))
					{
						return RedirectToAction("Index", "Home");
					}
					return RedirectToLocal(returnUrl);
				}
				else
				{
					await UserManager.AccessFailedAsync(user.Id);
					if (await UserManager.IsLockedOutAsync(user.Id))
					{
						await HandleAccountLockout(user);
						return View(model);
					}
					ModelState.AddModelError("", string.Format(Resources.Authorization.loginFailedError, UserManager.MaxFailedAccessAttemptsBeforeLockout - user.AccessFailedCount));
					await SendEmailAsync(EmailTemplateType.LogonFail, null, user.Email, user.Id, user.UserName, user.AccessFailedCount, UserManager.MaxFailedAccessAttemptsBeforeLockout - user.AccessFailedCount);
					return View(model);
				}
			}

			// If we got this far, something failed, redisplay form
			ModelState.AddModelError("", Resources.Authorization.loginFailedError);
			return View(model);
		}

		/// <summary>
		/// POST: LogOff.
		/// </summary>
		/// <returns></returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult LogOff()
		{
			AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie, DefaultAuthenticationTypes.ApplicationCookie, DefaultAuthenticationTypes.TwoFactorCookie);
			UserManager.UpdateSecurityStamp(User.Identity.GetUserId());
			return RedirectToAction("Index", "Home");
		}

		[HttpGet]
		[Authorize]
		public ActionResult LogOut()
		{
			AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie, DefaultAuthenticationTypes.ApplicationCookie, DefaultAuthenticationTypes.TwoFactorCookie);
			UserManager.UpdateSecurityStamp(User.Identity.GetUserId());
			return RedirectToAction("Index", "Home");
		}

		/// <summary>
		/// GET: Verifies the login two factor code.
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[AllowAnonymous]
		public async Task<ActionResult> VerifyLoginTwoFactor()
		{
			var userid = await GetTwoFactorLoginUserIdAsync();
			if (string.IsNullOrEmpty(userid))
			{
				return RedirectToAction("Login");
			}

			var user = await UserManager.FindValidByIdAsync(userid);
			if (user == null)
			{
				return View("Error");
			}

			return View("VerifyLoginTwoFactor", user.GetTwoFactorModel<VerifyLoginTwoFactorModel>(TwoFactorComponent.Login));
		}


		/// <summary>
		/// POST: Verifies the login two factor code.
		/// </summary>
		/// <param name="model">The model.</param>
		[HttpPost]
		[AllowAnonymous]
		public async Task<ActionResult> VerifyLoginTwoFactor(VerifyLoginTwoFactorModel model)
		{
			if (!ModelState.IsValid)
			{
				return View("VerifyLoginTwoFactor", model);
			}

			string userId = await GetTwoFactorLoginUserIdAsync();
			if (userId == null)
			{
				return View("Error");
			}

			var user = await UserManager.FindValidByIdAsync(userId);
			if (user == null)
			{
				return View("Error");
			}

			//var logonTwoFactor = user.TwoFactor.FirstOrDefault(x => x.Component == TwoFactorComponent.Login);
			//if (logonTwoFactor == null)
			//{
			//	return View("Error");
			//}

			if (await UserManager.VerifyUserTwoFactorCodeAsync(TwoFactorComponent.Login, userId, model.Code1, model.Code2))
			{
				await SignInAsync(user, false);
				return RedirectToLocal("Home");
			}

			await UserManager.AccessFailedAsync(user.Id);
			if (await UserManager.IsLockedOutAsync(user.Id))
			{
				await HandleAccountLockout(user);
				return View("VerifyLoginTwoFactor", model);
			}

			ModelState.AddModelError("", "Invalid code");
			return View("VerifyLoginTwoFactor", model);
		}

		#endregion

		#region Register

		/// <summary>
		/// GET: Registers a new user.
		/// </summary>
		[HttpGet]
		[AllowAnonymous]
		public ActionResult Register(string referrer)
		{
			return View(new RegisterViewModel { Referrer = referrer });
		}

		/// <summary>
		/// POST: Registers a new user.
		/// </summary>
		/// <param name="model">The model.</param>
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Register(RegisterViewModel model)
		{
			if (!CryptopiaAuthenticationHelper.ValidateCaptcha())
			{
				ModelState.AddModelError("", Resources.Authorization.reCaptchaError);
				return View(model);
			}

			if (!ModelState.IsValid)
				return View(model);

			var existing = await UserManager.FindByHandleAsync(model.UserName);
			if (existing != null)
			{
				ModelState.AddModelError("", string.Format(Resources.Authorization.registerUserNameExistsError, model.UserName));
				return View(model);
			}

			var baseEmail = model.EmailAddress.Split('@');
			if (baseEmail != null && baseEmail.Any() && baseEmail.Count() == 2)
			{
				var cleanEmail = string.Format("{0}@{1}", baseEmail[0].Replace(".", ""), baseEmail[1]);
				var existinEmail = await UserManager.FindByEmailAsync(cleanEmail);
				if (existinEmail != null)
				{
					ModelState.AddModelError("", string.Format(Resources.Authorization.registerEmailExistsError, model.EmailAddress));
					return View(model);
				}
			}

			var user = new ApplicationUser
			{
				UserName = model.UserName,
				Email = model.EmailAddress,
				ChatHandle = model.UserName,
				MiningHandle = model.UserName,
				RegisterDate = DateTime.UtcNow,
				Referrer = string.IsNullOrEmpty(model.Referrer) ? "System" : model.Referrer,
				DisableWithdrawEmailConfirmation = false,
				DisableRewards = true,
				DisableLogonEmail = true,
				IsUnsafeWithdrawEnabled = true,
				VerificationLevel = VerificationLevel.Level1
			};
			user.Settings = new UserSettings
			{
				HideZeroBalance = false,
				ShowFavoriteBalance = false,
				Theme = SiteTheme.Light,
				Id = user.Id,
				DefaultMineShaft = 0,
				DefaultTradepair = 0
			};
			user.Profile = new UserProfile
			{
				Id = user.Id,
				IsPublic = false
			};
			user.TwoFactor = new List<UserTwoFactor>();
			foreach (TwoFactorComponent twoFactorComponent in Enum.GetValues(typeof(TwoFactorComponent)))
			{
				user.TwoFactor.Add(new UserTwoFactor
				{
					UserId = user.Id,
					Component = twoFactorComponent,
					Type = TwoFactorType.PinCode,
					IsEnabled = true,
					Data = model.PinCode
				});
			}

			var result = await UserManager.CreateAsync(user, model.Password);
			if (result.Succeeded)
			{
				string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
				var callbackUrl = Url.Action("RegisterConfirmEmail", "Login", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
				if (await SendEmailAsync(EmailTemplateType.Registration, null, user.Email, user.Id, user.UserName, callbackUrl))
				{
					return ViewMessage(new ViewMessageModel(ViewMessageType.Info, Resources.Authorization.registerEmailConfirmationTitle, string.Format(Resources.Authorization.registerEmailConfirmationMessage, user.Email)));
				}

				ModelState.AddModelError("", Resources.Authorization.registerEmailConfirmationError);
			}

			AddErrors(result);
			return View(model);
		}

		/// <summary>
		/// GET: Confirm the registration token emailed to the user email.
		/// </summary>
		/// <param name="userId">The user identifier.</param>
		/// <param name="code">The code.</param>
		[HttpGet]
		[AllowAnonymous]
		public async Task<ActionResult> RegisterConfirmEmail(string userId, string code)
		{
			if (userId == null || code == null)
			{
				return Unauthorized();
			}

			if (await UserManager.FindByIdAsync(userId) != null)
			{
				var result = await UserManager.ConfirmEmailAsync(userId, code);
				if (result.Succeeded)
				{
					await UserSyncService.SyncUser(userId);
					return
						ViewMessage(new ViewMessageModel(ViewMessageType.Success, Resources.Authorization.registerEmailConfirmedTitle,
							Resources.Authorization.registerEmailConfirmedMessage, showLoginLink: true));
				}
			}

			return Unauthorized();
		}

		#endregion

		#region Lock/Unlock Account

		/// <summary>
		/// GET: Locks the users account.
		/// </summary>
		/// <param name="code">The code.</param>
		[HttpGet]
		[AllowAnonymous]
		public async Task<ActionResult> LockAccount(string userId, string token)
		{
			var user = await UserManager.FindValidByIdAsync(userId);
			if (user == null || string.IsNullOrEmpty(token))
			{
				return RedirectToAction("Index", "Home");
			}

			if (await VerifyUserTwoFactorTokenAsync(TwoFactorTokenType.LockAccount, userId, token, true))
			{
				if (!await UserManager.IsLockedOutAsync(userId))
				{
					await UserManager.SetLockoutEndDateAsync(userId, DateTime.UtcNow.AddDays(1));
					await UserManager.UpdateSecurityStampAsync(userId);
					if (user.TwoFactor.Any(x => x.Component == TwoFactorComponent.Lockout))
					{
						var unlocktoken = Url.Action("UnlockAccount", "Login",
							new { userId = user.Id, token = await GenerateUserTwoFactorTokenAsync(TwoFactorTokenType.UnlockAccount, user.Id) },
							protocol: Request.Url.Scheme);
						await SendEmailAsync(EmailTemplateType.LockoutByUserWithReset, null, user.Email, user.Id, user.UserName, unlocktoken);
						return
							ViewMessage(new ViewMessageModel(ViewMessageType.Warning, Resources.Authorization.lockByUserTitle,
								Resources.Authorization.lockByUserWithResetMessage));
					}
				}

				await SendEmailAsync(EmailTemplateType.LockoutByUser, null, user.Email, user.Id, user.UserName);
				return
					ViewMessage(new ViewMessageModel(ViewMessageType.Warning, Resources.Authorization.lockByUserTitle,
						Resources.Authorization.lockByUserMessage, showSupportLink: true));
			}
			return RedirectToAction("Index", "Home");
		}

		/// <summary>
		/// GET: Verifies the unlock account code from the users email link.
		/// </summary>
		/// <param name="userId">The user identifier.</param>
		/// <param name="token">The token sent in the email.</param>
		[HttpGet]
		[AllowAnonymous]
		public async Task<ActionResult> UnlockAccount(string userId, string token)
		{
			if (!await VerifyUserTwoFactorTokenAsync(TwoFactorTokenType.UnlockAccount, userId, token, true))
			{
				return RedirectToAction("Index", "Home");
			}

			var user = await UserManager.FindByIdAsync(userId);
			if (user == null)
			{
				return RedirectToAction("Index", "Home");
			}

			return View("VerifyLockoutTwoFactor", user.GetTwoFactorModel<VerifyLockoutTwoFactorModel>(TwoFactorComponent.Lockout));
		}

		/// <summary>
		/// POST: Verifies the unlock account code.
		/// </summary>
		/// <param name="model">The model.</param>
		[HttpPost]
		[AllowAnonymous]
		public async Task<ActionResult> VerifyLockoutTwoFactor(VerifyLockoutTwoFactorModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var user = await UserManager.FindByEmailAsync(model.EmailAddress);
			if (user == null)
			{
				return RedirectToAction("Login");
			}

			var lockoutTwoFactor = user.TwoFactor.FirstOrDefault(x => x.Component == TwoFactorComponent.Lockout);
			if (lockoutTwoFactor == null)
			{
				return
					ViewMessage(new ViewMessageModel(ViewMessageType.Warning, Resources.Authorization.unlockTwoFactorNotSetTitle,
						Resources.Authorization.unlockTwoFactorNotSetMessage, showSupportLink: true));
			}

			if (await UserManager.VerifyUserTwoFactorCodeAsync(TwoFactorComponent.Lockout, user.Id, model.Code1, model.Code2))
			{
				user.LockoutEndDateUtc = null;
				user.AccessFailedCount = 0;
				await UserManager.UpdateAsync(user);
				await UserManager.UpdateSecurityStampAsync(user.Id);
				return
					ViewMessage(new ViewMessageModel(ViewMessageType.Success, Resources.Authorization.unlockTitle,
						Resources.Authorization.unlockMessage, true));
			}

			return
				ViewMessage(new ViewMessageModel(ViewMessageType.Danger, Resources.Authorization.unlockTwoFactorFailedTitle, Resources.Authorization.unlockTwoFactorFailedMessage));
		}

		#endregion

		#region Forgot/Reset Password

		/// <summary>
		/// GET: Forgot password view.
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[AllowAnonymous]
		public ActionResult ForgotPassword()
		{
			return View();
		}

		/// <summary>
		/// POST: Forgot password. sends a secure token link to users email
		/// </summary>
		/// <param name="model">The model.</param>
		/// <returns></returns>
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
		{
			if (!CryptopiaAuthenticationHelper.ValidateCaptcha())
			{
				ModelState.AddModelError("", Resources.Authorization.reCaptchaError);
				return View(model);
			}

			if (!ModelState.IsValid)
			{
				return View(model);
			}


			if (ModelState.IsValid)
			{
				var message = new ViewMessageModel(ViewMessageType.Info, Resources.Authorization.resetTitle,
					Resources.Authorization.resetMessage);
				var user = await UserManager.FindByEmailAsync(model.Email);
				if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)) || user.IsDisabled)
				{
					// Don't reveal that the user does not exist or is not confirmed
					return ViewMessage(message);
				}

				var resetPasswordToken = Url.Action("ResetPassword", "Login",
					new { code = await UserManager.GeneratePasswordResetTokenAsync(user.Id) }, protocol: Request.Url.Scheme);
				await SendEmailAsync(EmailTemplateType.PasswordReset, null, user.Email, user.Id, user.UserName, resetPasswordToken);
				return ViewMessage(message);
			}

			// If we got this far, something failed, redisplay form
			return View(model);
		}

		/// <summary>
		/// Forgots the password confirmation.
		/// </summary>
		/// <returns></returns>
		//[HttpGet]
		//[AllowAnonymous]
		//public ActionResult ForgotPasswordConfirmation()
		//{
		//	return View();
		//}
		/// <summary>
		/// GET: Resets the password with the code from the link sent to the user.
		/// </summary>
		/// <param name="code">The code.</param>
		[HttpGet]
		[AllowAnonymous]
		public ActionResult ResetPassword(string code)
		{
			return View("ResetPassword", new ResetPasswordViewModel { Code = code });
		}

		/// <summary>
		/// Resets the users password.
		/// </summary>
		/// <param name="model">The model.</param>
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var user = await UserManager.FindByEmailAsync(model.Email);
			if (user == null)
			{
				// Don't reveal that the user does not exist
				return RedirectToAction("ResetPasswordConfirmation", "Login");
			}
			var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
			if (result.Succeeded)
			{
				await UserManager.UpdateSecurityStampAsync(user.Id);
				return RedirectToAction("ResetPasswordConfirmation", "Login");
			}
			AddErrors(result);
			return View();
		}

		/// <summary>
		/// GET: Password reset confirmation view
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[AllowAnonymous]
		public ActionResult ResetPasswordConfirmation()
		{
			return ViewMessage(new ViewMessageModel(ViewMessageType.Success, Resources.Authorization.resetConfirmedTitle,
				Resources.Authorization.resetConfirmedMessage, showLoginLink: true));
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Gets the authentication manager.
		/// </summary>
		private IAuthenticationManager AuthenticationManager
		{
			get { return HttpContext.GetOwinContext().Authentication; }
		}

		/// <summary>
		/// Signs the user in asynchronously.
		/// </summary>
		/// <param name="user">The user.</param>
		/// <param name="isPersistent">if set to <c>true</c> [is persistent].</param>
		/// <returns></returns>
		private async Task SignInAsync(ApplicationUser user, bool isPersistent)
		{
			AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie, DefaultAuthenticationTypes.ApplicationCookie, DefaultAuthenticationTypes.TwoFactorCookie);
			var identity = await user.GenerateUserIdentityAsync(UserManager);
			AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = true }, identity);

			// Log IP address
			user.Logons.Add(new UserLogon(Request.GetIPAddress()));
			await UserManager.UpdateAsync(user);
			if (!user.DisableLogonEmail)
			{
				await this.SendEmailAsync(EmailTemplateType.LogonSuccess, null, user.Email, user.Id, user.UserName);
			}
		}

		/// <summary>
		/// Sets the two factor login authentication cookie.
		/// </summary>
		/// <param name="userId">The user identifier.</param>
		private void SetTwoFactorLoginAuthCookie(string userId)
		{
			var identity = new ClaimsIdentity(DefaultAuthenticationTypes.TwoFactorCookie);
			identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, userId));
			AuthenticationManager.SignIn(identity);
		}

		/// <summary>
		/// Gets the two factor login user identifier asynchronous.
		/// </summary>
		/// <returns></returns>
		private async Task<string> GetTwoFactorLoginUserIdAsync()
		{
			var result = await AuthenticationManager.AuthenticateAsync(DefaultAuthenticationTypes.TwoFactorCookie);
			if (result != null && result.Identity != null && !string.IsNullOrEmpty(result.Identity.GetUserId()))
			{
				return result.Identity.GetUserId();
			}
			return null;
		}

		/// <summary>
		/// Handles the account lockout.
		/// </summary>
		/// <param name="user">The user.</param>
		private async Task HandleAccountLockout(ApplicationUser user)
		{
			if (user.TwoFactor.Any(x => x.Component == TwoFactorComponent.Lockout))
			{
				var token = Url.Action("UnlockAccount", "Login", new { userId = user.Id, token = await GenerateUserTwoFactorTokenAsync(TwoFactorTokenType.UnlockAccount, user.Id) }, protocol: Request.Url.Scheme);
				ModelState.AddModelError("", string.Format(Resources.Authorization.loginLockError, UserManager.MaxFailedAccessAttemptsBeforeLockout));
				ModelState.AddModelError("", Resources.Authorization.loginLockEmailError);
				await SendEmailAsync(EmailTemplateType.LockoutWithReset, null, user.Email, user.Id, user.UserName, token);
			}
			else
			{				                            
				ModelState.AddModelError("", string.Format(Resources.Authorization.loginLockForHoursError, UserManager.DefaultAccountLockoutTimeSpan.TotalHours, UserManager.MaxFailedAccessAttemptsBeforeLockout));
				await SendEmailAsync(EmailTemplateType.Lockout, null, user.Email, user.Id, user.UserName);
			}
		}

		/// <summary>
		/// Adds the errors to the currenct ModelState.
		/// </summary>
		/// <param name="result">The result.</param>
		private void AddErrors(IdentityResult result)
		{
			foreach (var error in result.Errors)
			{
				if (error == "Passwords must have at least one non letter or digit character.")
					ModelState.AddModelError("Error", "Passwords must have at least one non letter/digit character. e.g: !@#$^&()_");
				else
					ModelState.AddModelError("Error", error);
			}
		}

		/// <summary>
		/// Redirects to local.
		/// </summary>
		/// <param name="returnUrl">The return URL.</param>
		/// <returns></returns>
		private ActionResult RedirectToLocal(string returnUrl)
		{
			if (Url.IsLocalUrl(returnUrl))
			{
				return Redirect(returnUrl);
			}
			return RedirectToAction("Index", "Home");
		}

		#endregion
	}
}