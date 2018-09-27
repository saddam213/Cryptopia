using System;
using System.IO;
using System.Security.Principal;
using System.Web.Hosting;
using Microsoft.AspNet.Identity;
using System.Linq;
using System.Data.Entity;
using System.Collections.Generic;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Web;
using System.Net;
using System.Threading.Tasks;
using Cryptopia.Common.TwoFactor;
using Cryptopia.Entity;
using Cryptopia.Enums;
using Web.Site.Helpers;

namespace Web.Site
{
	public static class AspIdentityExtensions
	{
		public static string GetAvatar(this IIdentity identity)
		{
			if (identity != null && identity.IsAuthenticated)
			{
				string serverPath = HostingEnvironment.MapPath("~/Content/Images/Avatar");
				string filename = string.Format("{0}\\{1}", serverPath, identity.Name);
				if (File.Exists(filename + ".png"))
				{
					return string.Format("{0}/Content/Images/Avatar/{1}.png", CdnHelper.ImagePath(), identity.Name);
				}
			}
			return CdnHelper.ImagePath() + "/Content/Images/Avatar.png";
		}

		public static async Task<ApplicationUser> FindByHandleAsync(this UserManager<ApplicationUser, string> userManager, string name)
		{
			var user = await userManager.FindByNameAsync(name);
			if (user == null)
			{
				user = await userManager.Users.FirstOrDefaultAsync(x => x.ChatHandle == name);
				if (user == null)
				{
					user = await userManager.Users.FirstOrDefaultAsync(x => x.MiningHandle == name);
				}
			}
			return user;
		}

		public static async Task<ApplicationUser> FindValidByIdAsync(this UserManager<ApplicationUser> userManager, string userId)
		{
			var user = await userManager.FindByIdAsync(userId);
			if (user != null)
			{
				if (user.LockoutEndDateUtc.HasValue && user.LockoutEndDateUtc > DateTime.UtcNow)
				{
					var authMgr = HttpContext.Current.GetOwinContext().Authentication;
					if (authMgr != null)
					{
						authMgr.SignOut(DefaultAuthenticationTypes.ExternalCookie);
						authMgr.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
					}
					return null;
				}
				return user;
			}
			return null;
		}

		public static T GetTwoFactorModel<T>(this ApplicationUser user, TwoFactorComponent component) where T : IVerifyTwoFactorModel, new()
		{
			if (user.TwoFactor == null)
			{
				return new T { Type = TwoFactorType.None };
			}

			var result = user.TwoFactor.FirstOrDefault(x => x.Component == component);
			if (result == null)
			{
				return new T { Type = TwoFactorType.None };
			}

			var model = new T();
			model.Type = result.Type;

			if (model.Type == TwoFactorType.Question)
			{
				model.Question1 = result.Data;
				model.Question2 = result.Data3;
			}

			return model;
		}

		public static T GetTwoFactorModel<T>(this ApplicationUser user, TwoFactorComponent component, T existing) where T : IVerifyTwoFactorModel
		{
			if (user.TwoFactor == null)
			{
				existing.Type = TwoFactorType.None;
				return existing;
			}

			var result = user.TwoFactor.FirstOrDefault(x => x.Component == component);
			if (result == null)
			{
				existing.Type = TwoFactorType.None;
				return existing;
			}


			existing.Type = result.Type;
			if (existing.Type == TwoFactorType.Question)
			{
				existing.Question1 = result.Data;
				existing.Question2 = result.Data3;
			}

			return existing;
		}


		public static string GetIPAddress(this HttpRequestBase request)
		{
			try
			{
				string szRemoteAddr = request.UserHostAddress;
				string sForwardedFor = request.Headers["X-Forwarded-For"];
				if (!string.IsNullOrEmpty(sForwardedFor))
				{
					if (sForwardedFor.Contains(':'))
					{
						var ipAddress = "0.0.0.0";
						if (sForwardedFor.Contains("]"))
							ipAddress = sForwardedFor.Split(']')[0].TrimStart('[');
						else
							ipAddress = sForwardedFor.Split(':')[0];

						return ipAddress;
					}
					return sForwardedFor;
				}
				return szRemoteAddr;
			}
			catch (Exception)
			{
				return "0.0.0.0";
			}
		}

		private static bool IsLocalHost(string input)
		{
			IPAddress[] host;
			//get host addresses
			try
			{
				host = Dns.GetHostAddresses(input);
			}
			catch (Exception)
			{
				return false;
			}
			//get local adresses
			IPAddress[] local = Dns.GetHostAddresses(Dns.GetHostName());
			//check if local
			return host.Any(hostAddress => IPAddress.IsLoopback(hostAddress) || local.Contains(hostAddress));
		}
	}

	public enum CryptopiaRole
	{
		User = 0,
		Admin = 1,
		Moderator = 2
	}
}