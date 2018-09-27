using System;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web;
using Cryptopia.Base.Logging;
using Cryptopia.Data.DataContext;
using Cryptopia.Entity;
using Cryptopia.Enums;
using Cryptopia.Infrastructure.Common.DataContext;
using hbehr.recaptcha;
using ITSecurity;

namespace Cryptopia.Infrastructure.Helpers
{
	public class CryptopiaAuthenticationHelper
	{
		private static readonly Log Log = LoggingManager.GetLog(typeof(GoogleAuthenticationHelper));

		public static async Task<bool> VerifyTwoFactorCode(string userId, string code)
		{
			var exchangeDataContextFactory = new DataContextFactory();

			try
			{
				using (var context = exchangeDataContextFactory.CreateContext())
				{
					var userTwofactor = await context.TwoFactorCode.FirstOrDefaultAsync(x => x.UserId == userId);
					return await VerifyTwoFactorCode(userTwofactor, code, context);
				}
			}
			catch (Exception ex)
			{
				Log.Exception("[VerifyTwoFactorCode] Code Verification Failed", ex);
			}
			return false;
		}

		public static async Task<bool> VerifyTwoFactorCode(AuthenticatedFeatureType authenticatedFeatureType, string code)
		{
			var exchangeDataContextFactory = new DataContextFactory();

			try
			{
				using (var context = exchangeDataContextFactory.CreateContext())
				{
					var authenticatedFeature = await context.AuthenticatedFeature.FirstOrDefaultAsync(x => x.AuthenticatedFeatureType == authenticatedFeatureType);
					var userTwofactor = authenticatedFeature.TwoFactorCode;
					return await VerifyTwoFactorCode(userTwofactor, code, context);
				}
			}
			catch (Exception ex)
			{
				Log.Exception("[VerifyTwoFactorCode] Code Verification Failed", ex);
			}
			return false;
		}

		public static async Task<bool> VerifyTwoFactorCode(TwoFactorCode twoFactorCode, string code, IDataContext context)
		{
			if (twoFactorCode == null)
				return false;

			var key = new seamoonapi();
			var result = key.passwordsyn(twoFactorCode.CurrentData ?? twoFactorCode.OriginalData, code);

			if (result.Length <= 3) return false;

			twoFactorCode.CurrentData = result;
			await context.SaveChangesAsync();
			return true;
		}

		public static bool ValidateCaptcha()
		{
			try
			{
#if !DEBUG
				return ReCaptcha.ValidateCaptcha(HttpContext.Current.Request.Params["g-recaptcha-response"]);
#endif
			}
			catch (Exception ex)
			{
				// We don't return false here because we don't want to lock out users if there are issues with google and not the site
				Log.Exception("[ValidateCaptcha] Captcha Verification Failed", ex);
			}
			return true;
		}
	}
}