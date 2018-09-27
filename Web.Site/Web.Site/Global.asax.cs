using System;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Web.Api;
using Microsoft.AspNet.Identity;
using Web.Site.Notifications;
using Cryptopia.DependencyInjection;
using Web.Site.ModelBinders;
using Cryptopia.Infrastructure.Common.DataTables;
using Cryptopia.Common.Validation;
using hbehr.recaptcha;
using Web.Site.Helpers;

namespace Web.Site
{
	public class MvcApplication : System.Web.HttpApplication
	{
		protected void Application_Start()
		{
			System.Net.ServicePointManager.DefaultConnectionLimit = int.MaxValue;
			MvcHandler.DisableMvcResponseHeader = true;
			ViewEngines.Engines.Clear();
			ViewEngines.Engines.Add(new RazorViewEngine());
			DependencyRegistrar.Register("Web.Site");
			//DependencyRegistrar.Container
			AreaRegistration.RegisterAllAreas();
			GlobalConfiguration.Configure(WebApiConfig.Register);
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);
			DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(RangeBaseAttribute), typeof(RangeAttributeAdapter));
			DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(StringLengthBaseAttribute), typeof(StringLengthAttributeAdapter));
			DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(RequiredBaseAttribute), typeof(RequiredAttributeAdapter));
			DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof (RequiredIfAttribute), typeof (RequiredAttributeAdapter));
			DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof (RequiredToBeTrueAttribute), typeof (RequiredAttributeAdapter));
			ModelMetadataProviders.Current = new CachedDataAnnotationsModelMetadataProvider();
			System.Web.Mvc.ModelBinders.Binders.Add(typeof (DataTablesModel), new DataTablesModelBinder());
			//NotificationHub.InitializeChat();
			
			string publicKey = WebConfigurationManager.AppSettings["RecaptchaPublicKey"];
			string secretKey = WebConfigurationManager.AppSettings["RecaptchaPrivateKey"];
			ReCaptcha.Configure(publicKey, secretKey);

			//
			Localization.Init(WebConfigurationManager.AppSettings["Localization_EnabledLanguages"]);			
		}

		protected void Application_BeginRequest(object sender, EventArgs e)
		{
			Localization.InitRequest(Context);
		}

		//string arg filled with the value of "varyByCustom" in your web.config
		public override string GetVaryByCustomString(HttpContext context, string arg)
		{
			if (arg == "User")
			{
				return context.User.Identity.GetUserId() ?? Guid.Empty.ToString();
			}
			return base.GetVaryByCustomString(context, arg);
		}

		protected void Application_End()
		{
			DependencyRegistrar.Deregister();
		}

		protected void Application_Error(object sender, EventArgs e)
		{
			var exception = Server.GetLastError();
			var httpException = exception as HttpException;
			// As part of Bad Request errors can be "potentialy dangerous" requests which we can't route to .aspx.
			// Same exception would be thrown. So lets pass it to IIS which will present our static 400 custom err for us.
			if (httpException?.GetHttpCode() == 400)
			{
				Server.ClearError();
				Response.Clear();
				Response.StatusCode = 400;
			}
		}
	}
}