using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System;
using Web.Site.App_Start;
using Web.Site.Notifications;
using System.Web;
using Cryptopia.Data.DataContext;
using Cryptopia.Entity;
using System.Web.Configuration;
using Cryptopia.Cache;

namespace Web.Site
{
	public partial class Startup
	{
		// For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
		private static void ConfigureAuth(IAppBuilder app)
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
					OnApplyRedirect = ctx =>
					{
						if (!IsApiRequest(ctx.Request))
						{
							ctx.Response.Redirect(ctx.RedirectUri);
						}
					}
				},
#if !DEBUG
				CookieSecure = CookieSecureOption.Always,
#endif
			  CookieName = "Cryptopia",
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

			var useRedisSignalr = bool.Parse(WebConfigurationManager.AppSettings["Signalr_ScaleOut_Redis_Enabled"]);
			if (useRedisSignalr)
			{
				var redisConnection = WebConfigurationManager.AppSettings["Signalr_ScaleOut_Redis_Connection"];
				var redisConfig = new RedisScaleoutConfiguration(redisConnection, "Cryptopia.Signalr");
				redisConfig.MaxQueueLength = int.MaxValue;
				redisConfig.QueueBehavior = QueuingBehavior.Always;
				GlobalHost.DependencyResolver.UseRedis(redisConfig);
			}

			var useSqlSignalr = bool.Parse(WebConfigurationManager.AppSettings["Signalr_ScaleOut_Sql_Enabled"]);
			if (useSqlSignalr)
			{
				var sqlConnection = WebConfigurationManager.ConnectionStrings["SignalrBackplane"].ConnectionString;
				var sqlConfig = new SqlScaleoutConfiguration(sqlConnection);
				sqlConfig.TableCount = int.Parse(WebConfigurationManager.AppSettings["Signalr_ScaleOut_Sql_TableCount"]);
				sqlConfig.MaxQueueLength = int.MaxValue;
				sqlConfig.QueueBehavior = QueuingBehavior.Always;
				GlobalHost.DependencyResolver.UseSqlServer(sqlConfig);
			}

			GlobalHost.DependencyResolver.Register(typeof(IUserIdProvider), () => new HubIdentityProvider());
			GlobalHost.Configuration.DefaultMessageBufferSize = 200;
			app.MapSignalR(hubConfiguration);
		}

		private static bool IsApiRequest(IOwinRequest request)
		{
			string apiPath = VirtualPathUtility.ToAbsolute("~/Api/");
			return request.Uri.LocalPath.StartsWith(apiPath, StringComparison.OrdinalIgnoreCase);
		}
	}
}