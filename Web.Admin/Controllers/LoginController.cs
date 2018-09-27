using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Web.Admin.Models;
using Cryptopia.Enums;
using Cryptopia.Data.DataContext;
using Cryptopia.Entity;
using Cryptopia.Common.User;
using Web.Admin.Helpers;
using hbehr.recaptcha;
using System.Data.Entity;
using Cryptopia.Infrastructure.Helpers;

namespace Web.Admin.Controllers
{
	[Authorize]
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
#if !DEBUG
			if (!CryptopiaAuthenticationHelper.ValidateCaptcha())
			{
				ModelState.AddModelError("", "Invalid reCaptcha");
				return View(model);
			}

			if (!ModelState.IsValid)
			{
				return View(model);
			}
#endif

			var user = await UserManager.FindByEmailAsync(model.EmailAddress);
			if (user == null || user.IsDisabled || !await UserManager.IsInRoleAsync(user.Id, "Admin") || !await UserManager.CheckPasswordAsync(user, model.Password))
			{
				ModelState.AddModelError("", "Email or password was invalid.");
				return View(model);
			}

			// is the user locked out
			if (await UserManager.IsLockedOutAsync(user.Id))
			{
				ModelState.AddModelError("", "Locked Out.");
				return View(model);
			}

			await UserSyncService.SyncUser(user.Id);
			await UserManager.ResetAccessFailedCountAsync(user.Id);

#if !DEBUG
			if(!await CryptopiaAuthenticationHelper.VerifyTwoFactorCode(user.Id, model.TwoFactor))
			{
				ModelState.AddModelError("", "Two factor token incorrect.");
				return View(model);
			}
#endif

			await SignInAsync(user, false);
			if (string.IsNullOrEmpty(returnUrl))
			{
				return RedirectToAction("Index", "Support");
			}
			return RedirectToLocal(returnUrl);
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
			return RedirectToAction("Index", "Support");
		}

		[HttpGet]
		[Authorize]
		public ActionResult LogOut()
		{
			AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie, DefaultAuthenticationTypes.ApplicationCookie, DefaultAuthenticationTypes.TwoFactorCookie);
			UserManager.UpdateSecurityStamp(User.Identity.GetUserId());
			return RedirectToAction("Index", "Support");
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
				ModelState.AddModelError("", string.Format("Your account has been locked due to {0} invalid logon attempts.", UserManager.MaxFailedAccessAttemptsBeforeLockout));
				ModelState.AddModelError("", "An email with unlock instructions has been sent to your registered email address.");
				await SendEmailAsync(EmailTemplateType.LockoutWithReset, null, user.Email, user.Id, user.UserName, token);
			}
			else
			{
				ModelState.AddModelError("", string.Format("Your account has been locked for {0} hours due to {1} invalid logon attempts.", UserManager.DefaultAccountLockoutTimeSpan.TotalHours, UserManager.MaxFailedAccessAttemptsBeforeLockout));
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
			return RedirectToAction("Index", "Support");
		}

#endregion
	}
}