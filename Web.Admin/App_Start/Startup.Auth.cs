using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using Web.Admin.Models;
using Microsoft.AspNet.SignalR;
using Web.Admin.Hubs;
using Cryptopia.Data.DataContext;
using Web.Admin.Identity;
using Cryptopia.Entity;

namespace Web.Admin
{
	public partial class Startup
	{
		// For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
		public void ConfigureAuth(IAppBuilder app)
		{
			app.CreatePerOwinContext(ApplicationDbContext.Create);
			app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);

			// Enable the application to use a cookie to store information for the signed in user
			app.UseCookieAuthentication(new CookieAuthenticationOptions
			{
				AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
				Provider = new CookieAuthenticationProvider
				{
					// Enables the application to validate the security stamp when the user logs in.
					// This is a security feature which is used when you change a password or add an external login to your account.  
					OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
																			 validateInterval: TimeSpan.FromMinutes(2),
																			 regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager)),
					OnApplyRedirect = ctx => ctx.Response.Redirect(ctx.RedirectUri)
				},
#if !DEBUG
				CookieSecure = CookieSecureOption.Always,
#endif
				CookieName = "CryptopiaAdmin",
				LoginPath = new PathString("/Login"),
				ExpireTimeSpan = TimeSpan.FromHours(8),
				LogoutPath = new PathString("/LogOff"),

			});
			// Use a cookie to temporarily store information about a user logging in with a third party login provider
			//app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);
			app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));

			// Signalr
			var hubConfiguration = new HubConfiguration
			{
#if DEBUG
				EnableDetailedErrors = true,
#else
				EnableDetailedErrors = false
#endif
			};
			GlobalHost.DependencyResolver.Register(typeof(IUserIdProvider), () => new HubIdentityProvider());
			app.MapSignalR(hubConfiguration);
		}
	}
}