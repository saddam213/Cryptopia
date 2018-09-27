using Cryptopia.Enums;
using Microsoft.Owin.Security;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;

namespace Web.Site.Helpers
{

	public static class CryptopiaClaims
	{

		public static void UpdateClaim(this IPrincipal principal, string claimType, string value)
		{
			var claimsIdentity = principal.Identity as ClaimsIdentity;
			if (claimsIdentity != null)
			{
				var claim = claimsIdentity.Claims.FirstOrDefault(x => x.Type == claimType);
				if (claim != null)
				{
					// If the value is the same, just return
					if (claim.Value == value)
						return;

					claimsIdentity.RemoveClaim(claim);
				}

				claimsIdentity.AddClaim(new Claim(claimType, value));
				var authenticationManager = HttpContext.Current.GetOwinContext().Authentication;
				authenticationManager.AuthenticationResponseGrant = new AuthenticationResponseGrant(new ClaimsPrincipal(claimsIdentity), new AuthenticationProperties() { IsPersistent = true });
			}
		}

		public static string GetClaimValue(this IPrincipal principal, string key, string defaultValue = null)
		{
			var identity = principal.Identity as ClaimsIdentity;
			if (identity == null)
				return defaultValue;

			var claim = identity.Claims.FirstOrDefault(c => c.Type == key);
			if (claim == null)
				return defaultValue;

			return claim.Value;
		}

	}

	public class ClaimsUser : ClaimsPrincipal
	{
		public ClaimsUser(ClaimsPrincipal principal)
				: base(principal)
		{
		}

		public string Theme
		{
			get
			{
				var theme = this.GetClaimValue(CryptopiaClaim.Theme, SiteTheme.Light.ToString());
				if (theme == "Cerulean" || theme == "Simplex" || theme == "Spacelab")
				{
					return SiteTheme.Light.ToString();
				}
				if (theme == "Cryptopia" || theme == "Slate" || theme == "Cyborg" || theme == "SuperHero")
				{
					return SiteTheme.Dark.ToString();
				}
				return theme;
			}
		}

		public string ChatHandle
		{
			get { return this.GetClaimValue(CryptopiaClaim.ChatHandle, string.Empty); }
		}

		public bool HideZeroBalance
		{
			get
			{
				var result = false;
				bool.TryParse(this.GetClaimValue(CryptopiaClaim.HideZeroBalance, false.ToString()), out result);
				return result;
			}
		}

		public bool ShowFavoriteBalance
		{
			get
			{
				var result = false;
				bool.TryParse(this.GetClaimValue(CryptopiaClaim.ShowFavoriteBalance, false.ToString()), out result);
				return result;
			}
		}

		public bool IsShareholder
		{
			get { return ShareCount > 0; }
		}

		public int ShareCount
		{
			get
			{
				var shares = 0;
				int.TryParse(this.GetClaimValue(CryptopiaClaim.Shareholder, "0"), out shares);
				return shares;
			}
		}

		private ChartSettings _chartSettings;
		public ChartSettings ChartSettings
		{
			get
			{
				if (_chartSettings == null)
					_chartSettings = new ChartSettings(this.GetClaimValue(CryptopiaClaim.ChartSettings, "1,0,1,0,0,0,0,0:50,0:30,0:20,100,0"));

				return _chartSettings;
			}
		}
	}
}


public class ChartSettings
{
	public ChartSettings()
	{

	}
	public ChartSettings(string config)
	{
		var options = config.Split(',').ToList();
		if (options.Count == 12)
		{
			if (options[0] == "1")
				Volume = true;
			if (options[1] == "1")
				StockPrice = true;
			if (options[2] == "1")
				Candlestick = true;
			if (options[3] == "1")
				MACD = true;
			if (options[4] == "1")
				Signal = true;
			if (options[5] == "1")
				Histogram = true;
			if (options[6] == "1")
				Fibonacci = true;

			if (options[7].StartsWith("1:"))
				SMA = true;
			if (options[8].StartsWith("1:"))
				EMA1 = true;
			if (options[9].StartsWith("1:"))
				EMA2 = true;

			var smaValue = 50;
			var ema1Value = 30;
			var ema2Value = 20;


			int.TryParse(options[7].Contains(":") ? options[7].Split(':')[1] : "50", out smaValue);
			int.TryParse(options[8].Contains(":") ? options[8].Split(':')[1] : "30", out ema1Value);
			int.TryParse(options[9].Contains(":") ? options[9].Split(':')[1] : "20", out ema2Value);

			SMAValue = smaValue;
			EMA1Value = ema1Value;
			EMA2Value = ema2Value;

			var orderbookPercent = 0;
			var distributionCount = 100;
			int.TryParse(options[10], out distributionCount);
			int.TryParse(options[11], out orderbookPercent);
			DistributionCount = distributionCount;
			OrderBookPercent = orderbookPercent;
		}
	}

	public bool Volume { get; set; } = true;
	public bool StockPrice { get; set; }
	public bool Candlestick { get; set; } = true;
	public bool MACD { get; set; }
	public bool Signal { get; set; }
	public bool Histogram { get; set; }
	public bool Fibonacci { get; set; }
	public bool SMA { get; set; }
	public int SMAValue { get; set; } = 50;
	public bool EMA1 { get; set; }
	public int EMA1Value { get; set; } = 30;
	public bool EMA2 { get; set; }
	public int EMA2Value { get; set; } = 20;
	public int DistributionCount { get; set; } = 100;
	public int OrderBookPercent { get; set; } = 0;
}