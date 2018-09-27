using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Web.Site.Models;
using Microsoft.AspNet.Identity.Owin;
using Web.Site.App_Start;
using System.Security.Claims;
using OtpSharp;
using Base32;
using System.Web.Configuration;
using Web.Site.ActionResults;
using Cryptopia.Enums;
using Cryptopia.Infrastructure.Common.DataTables;
using Web.Site.Helpers;
using Cryptopia.Infrastructure.Common.Email;

namespace Web.Site.Controllers
{
	public class BaseController : Controller
	{
		public IEmailService EmailService { get; set; }

		public ClaimsUser ClaimsUser
		{
			get { return new ClaimsUser(User as ClaimsPrincipal); }
		}

		protected override JsonResult Json(object data, string contentType, System.Text.Encoding contentEncoding, JsonRequestBehavior behavior)
		{
			return new JsonResult()
			{
				Data = data,
				ContentType = contentType,
				ContentEncoding = contentEncoding,
				JsonRequestBehavior = behavior,
				MaxJsonLength = Int32.MaxValue
			};
		}

		protected ActionResult JsonSuccess(string message = null)
		{
			var result = new JsonNetResult
			{
				Data = new { Success = true, Message = message }
			};
			return result;
		}

		protected ActionResult JsonError(string message = null)
		{
			var result = new JsonNetResult
			{
				Data = new { Success = false, Message = message }
			};
			return result;
		}

		protected ActionResult JsonResult(object data)
		{
			var result = new JsonNetResult
			{
				Data = data
			};
			return result;
		}

		protected DataTablesResult DataTable(DataTablesResponse dataTablesResponse)
		{
			return new DataTablesResult(dataTablesResponse);
		}

		#region Email

		protected async Task<bool> SendEmailAsync(ApplicationUserManager userManager, EmailTemplateType type, int? systemIdentifier, string destination, string userid, params object[] formatParameters)
		{
			var emailParameters = new List<object>();
			emailParameters.Add(Request.GetIPAddress()); // add ip address
			emailParameters.AddRange(formatParameters);
			emailParameters.Add(Url.Action("LockAccount", "Login", new { userId = userid, token = await userManager.GenerateUserTokenAsync("LockAccount", userid) }, protocol: Request.Url.Scheme));
			return await EmailService.SendEmail(new EmailMessageModel
			{
				EmailType = type,
				Destination = destination,
				SystemIdentifier = systemIdentifier,
				BodyParameters = emailParameters.ToArray()
			});
		}

		protected async Task<bool> SendSystemEmailAsync(EmailTemplateType type, SystemEmailType emailType, params object[] formatParameters)
		{
			return await EmailService.SendEmail(new EmailMessageModel
			{
				EmailType = type,
				Destination = WebConfigurationManager.AppSettings[emailType.ToString()],
				BodyParameters = formatParameters
			});
		}

		#endregion

		public ViewResult ViewMessage(ViewMessageModel model)
		{
			return View("ViewMessage", model);
		}

		public PartialViewResult PartialViewMessage(ViewMessageModel model)
		{
			return PartialView("ViewMessage", model);
		}

		public ViewResult ViewMessageModal(ViewMessageModel model)
		{
			return View("ViewMessageModal", model);
		}

		public ViewResult Unauthorized()
		{
			return View("Unauthorized");
		}

		public ViewResult UnauthorizedModal()
		{
			return ViewMessageModal(new ViewMessageModel(ViewMessageType.Info, Resources.Layout.unauthorizedLoginMessageTitle, Resources.Layout.unauthorizedLoginMessage, showLoginLink: true, returnUrl: Url.Action("Index")));
		}


		protected CloseModalResult CloseModal()
		{
			return new CloseModalResult();
		}

		protected CloseModalResult CloseModal(object data)
		{
			return new CloseModalResult(data);
		}

		protected CloseModalResult CloseModalSuccess(string message = null)
		{
			return new CloseModalResult(new { Success = true, Message = message });
		}

		protected CloseModalResult CloseModalError(string message = null)
		{
			return new CloseModalResult(new { Success = false, Message = message });
		}

		protected CloseModalRedirectResult CloseModalRedirect(string redirectAction)
		{
			return new CloseModalRedirectResult(redirectAction);
		}

		protected string GetLocalReturnUrl(string returnUrl)
		{
			var homeAction = Url.Action("Index", "Home");
			if (Url.IsLocalUrl(returnUrl))
			{
				if (returnUrl == "/" || returnUrl == homeAction)
					return string.Empty;

				return returnUrl;
			}
			return string.Empty;
		}
	}

	public class BaseUserController : BaseController
	{
		/// <summary>
		/// The user manager
		/// </summary>
		private ApplicationUserManager _userManager;

		public BaseUserController()
		{
		}

		private BaseUserController(ApplicationUserManager userManager)
		{
			UserManager = userManager;
		}

		/// <summary>
		/// Gets the user manager.
		/// </summary>
		public ApplicationUserManager UserManager
		{
			get { return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
			private set { _userManager = value; }
		}

		/// <summary>
		/// Generates unique hashed token based on the users security stamp to be sent via email.
		/// </summary>
		/// <param name="manager">The usermanager.</param>
		/// <param name="tokenType">Type of the token.</param>
		/// <param name="userid">The userid.</param>
		protected async Task<string> GenerateUserTwoFactorTokenAsync(TwoFactorTokenType tokenType, string userid)
		{
			return await UserManager.GenerateUserTokenAsync(tokenType.ToString(), userid);
		}

		/// <summary>
		/// Verifies the unique hashed token sent to the user
		/// </summary>
		/// <param name="manager">The manager.</param>
		/// <param name="tokenType">Type of the token.</param>
		/// <param name="userid">The userid.</param>
		/// <param name="code">The code.</param>
		/// <param name="updateSecurityStamp">if set to <c>true</c> update the users security stamp. This will invalidate any other active tokens</param>
		protected async Task<bool> VerifyUserTwoFactorTokenAsync(TwoFactorTokenType tokenType, string userid, string code,	bool updateSecurityStamp = false)
		{
			if (await UserManager.FindByIdAsync(userid) != null)
			{
				if (await UserManager.VerifyUserTokenAsync(userid, tokenType.ToString(), code))
				{
					// update security stamp, this will invalidate any other tokens
					if (updateSecurityStamp)
					{
						await UserManager.UpdateSecurityStampAsync(userid);
					}
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Generates a short security code for the user to manually enter.
		/// </summary>
		/// <param name="manager">The usermanager.</param>
		/// <param name="codeType">Type of the code.</param>
		/// <param name="userid">The userid.</param>
		/// <returns>A numeric code</returns>
		protected async Task<string> GenerateUserTwoFactorCodeAsync(TwoFactorType codeType, string userid,	bool updateSecurityStamp = false)
		{
			if (codeType == TwoFactorType.EmailCode)
			{
				if (updateSecurityStamp)
				{
					await UserManager.UpdateSecurityStampAsync(userid);
				}
				return await UserManager.GenerateTwoFactorTokenAsync(userid, codeType.ToString());
			}
			return string.Empty;
		}

		
		protected async Task<bool> SendEmailAsync(EmailTemplateType type, int? systemidentifier, string destination, string userid, params object[] formatParameters)
		{
			return await SendEmailAsync(UserManager, type, systemidentifier, destination, userid, formatParameters);
		}
	}
}