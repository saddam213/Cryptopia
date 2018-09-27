using Cryptopia.Common.Validation;
using Cryptopia.DependencyInjection;
using hbehr.recaptcha;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Cryptopia.Infrastructure.Common.DataTables;
using Web.Admin.ModelBinders;

namespace Web.Admin
{
	public class MvcApplication : System.Web.HttpApplication
	{
		protected void Application_Start()
		{
			MvcHandler.DisableMvcResponseHeader = true;
			ViewEngines.Engines.Clear();
			ViewEngines.Engines.Add(new RazorViewEngine());
			DependencyRegistrar.Register("Web.Admin");
			//DependencyRegistrar.Container
			AreaRegistration.RegisterAllAreas();
			//GlobalConfiguration.Configure(WebApiConfig.Register);
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);
			DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof (RequiredIfAttribute), typeof (RequiredAttributeAdapter));
			ModelMetadataProviders.Current = new CachedDataAnnotationsModelMetadataProvider();
			System.Web.Mvc.ModelBinders.Binders.Add(typeof (DataTablesModel), new DataTablesModelBinder());

			string publicKey = WebConfigurationManager.AppSettings["RecaptchaPublicKey"];
			string secretKey = WebConfigurationManager.AppSettings["RecaptchaPrivateKey"];
			ReCaptcha.Configure(publicKey, secretKey);
		}
	}
}