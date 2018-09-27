using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Web.Site.Helpers
{
	public static class Localization
	{
		const string CookieName = "CryptopiaLang";
		public static Dictionary<string, string> EnabledLanguages;
		private static Dictionary<string, string> SupportedLanguages = new Dictionary<string, string>() { 
			{ "en", "English" }, 
			{ "ru", "Русский" }
		};

		/// <summary>
		/// Two letter ISO language name of current thread culture as 
		/// </summary>
		public static string CurrentCulture
		{ 
			get { return Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName; }
		}

		/// <summary>
		/// Returns true if lang exists in list of enabled languages
		/// </summary>
		/// <param name="lang"></param>
		/// <returns></returns>
		public static bool IsValidName(string lang)
		{
			return EnabledLanguages.ContainsKey(lang);
		}

		public static void SetCookie(HttpContext context, string lang)
		{
			if (IsValidName(lang))
			{
				var cookie = new HttpCookie(CookieName, lang);
				cookie.Expires = DateTime.Now.AddYears(1);
				context.Response.Cookies.Add(cookie);
			}
		}

		/// <summary>
		/// Init localization 
		///  - set enabled languages
		/// </summary>
		/// <param name="enabledLanguages">Comma separated list of enabled languages, f.e. "en,ru"</param>
		public static void Init(string enabledLanguages)
		{
			enabledLanguages = enabledLanguages ?? "";
			var list = enabledLanguages
				.Split(',')
				.Where(l => l.Trim() != "")
				.Select(l => l.Trim().ToLower())				
				.ToArray();

			Init(list);
		}

		/// <summary>
		/// Init localization 
		///  - set enabled languages
		/// </summary>
		/// <param name="enabledLanguages"></param>
		public static void Init(string[] enabledLanguages = null)
		{
			//
			enabledLanguages = enabledLanguages ?? new string[] { };
			EnabledLanguages = SupportedLanguages
				.Where(l => enabledLanguages.Contains(l.Key))
				.ToDictionary(l => l.Key, l => l.Value);
		}

		/// <summary>
		/// Set current thread culture based on request cookie or http header values
		/// </summary>
		/// <param name="context"></param>
		public static void InitRequest(HttpContext context)
		{
			if (EnabledLanguages.Count == 0)
				return;

			//
			string cultureName = null;
			var cultureCookie = context.Request.Cookies[CookieName];
			if (cultureCookie != null && IsValidName(cultureCookie.Value))
				cultureName = cultureCookie.Value;

			// or from http header (browser preferences)
			if (cultureName == null && context.Request.UserLanguages != null)
			{
				foreach (var lang in context.Request.UserLanguages)
				{
					var name = lang.Substring(0, 2).ToLower();
					if (IsValidName(name))
					{
						cultureName = name;
						break;
					}
				}
			}

			// or default
			if (cultureName == null)
			{
				cultureName = EnabledLanguages.FirstOrDefault().Key;
			}

			// (re)set cookie 
			if (cultureCookie == null || cultureCookie.Value != cultureName || cultureCookie.Expires.AddMonths(1) > DateTime.Now)
			{
				SetCookie(context, cultureName);
			}

			// set response header
			context.Response.AddHeader("Content-language", cultureName);

			//
			var culture = CultureInfo.CreateSpecificCulture(cultureName);
			Thread.CurrentThread.CurrentCulture = culture;
			Thread.CurrentThread.CurrentUICulture = culture;					
		}
	}
}
