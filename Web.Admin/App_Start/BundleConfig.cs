using System.Web.Optimization;

namespace Web.Admin
{
	public class BundleConfig
	{
		// For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
		public static void RegisterBundles(BundleCollection bundles)
		{
			bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
				"~/Scripts/jquery-{version}.js",
				"~/Scripts/jquery.signalR-2.2.2.js"));

			bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
				"~/Scripts/jquery.validate.js",
				"~/Scripts/jquery.validate.unobtrusive.js"));

			// Use the development version of Modernizr to develop with and learn from. Then, when you're
			// ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
			bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
				"~/Scripts/modernizr-*"));

			bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
				"~/Scripts/bootstrap.js",
				"~/Scripts/respond.js"));

			bundles.Add(new ScriptBundle("~/bundles/site").Include(
				"~/Scripts/jquery.unobtrusive-ajax.js",
				"~/Scripts/jquery.dataTables.js",
				"~/Scripts/dataTables.bootstrap.js",
				"~/Scripts/select2.js",
				"~/Scripts/mustache.js",
				"~/Scripts/jquery.blockUI.js",
				"~/Scripts/simpleModal.js",
				"~/Scripts/moment.js",
				"~/Scripts/site.js"));

			bundles.Add(new StyleBundle("~/Content/css").Include(
				"~/Content/fontAwesome.css",
				"~/Content/bootstrap.css",
				"~/Content/select2.css",
				//"~/Content/jquery.dataTables.css",
				"~/Content/dataTables.bootstrap.css",
				"~/Content/Modal.css",
				"~/Content/site.css"));
		}
	}
}