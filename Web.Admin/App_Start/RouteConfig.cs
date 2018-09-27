using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Web.Admin
{
	public class RouteConfig
	{
		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			// Account
			routes.MapRoute(name: "Login", url: "Login", defaults: new { controller = "Login", action = "Login" });
			routes.MapRoute(name: "LogOff", url: "LogOff", defaults: new { controller = "Login", action = "LogOff" });

			routes.MapRoute(
					name: "Default",
					url: "{controller}/{action}/{id}",
					defaults: new { controller = "Support", action = "Index", id = UrlParameter.Optional }
			);
		}
	}
}
