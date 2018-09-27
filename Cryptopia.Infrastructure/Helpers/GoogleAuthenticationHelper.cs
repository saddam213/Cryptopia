using System;
using System.Web;
using Base32;
using Cryptopia.Base.Logging;
using Cryptopia.Common.TwoFactor;
using OtpSharp;

namespace Cryptopia.Infrastructure.Helpers
{
	public class GoogleAuthenticationHelper
	{
		private const string Issuer = "Cryptopia";
		private static readonly Log Log = LoggingManager.GetLog(typeof(GoogleAuthenticationHelper));

		public static GoogleTwoFactorData GetGoogleTwoFactorData(string userName)
		{
			try
			{
				var secretKey = KeyGeneration.GenerateRandomKey(20);
				var barcodeUrl = $"{KeyUrl.GetTotpUrl(secretKey, userName)}&issuer={Issuer}";
				return new GoogleTwoFactorData
				{
					PrivateKey = Base32Encoder.Encode(secretKey),
					PublicKey = HttpUtility.UrlEncode(barcodeUrl)
				};
			}
			catch (Exception ex)
			{
				Log.Exception("[GetGoogleTwoFactorData] Authentication Set Up Failed", ex);
			}
			return new GoogleTwoFactorData();
		}

		public static bool VerifyGoogleTwoFactorCode(string key, string code)
		{
			try
			{
				var secretKey = Base32Encoder.Decode(key);
				var otp = new Totp(secretKey);
                long _;
				return otp.VerifyTotp(code, out _, new VerificationWindow(5, 5));
			}
			catch (Exception ex)
			{
				Log.Exception("[VerifyGoogleTwoFactorCode] Verification Failed", ex);
			}
			return false;
		}
	}
}