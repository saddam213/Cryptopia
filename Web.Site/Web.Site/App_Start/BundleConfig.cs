using System.Web.Optimization;

namespace Web.Site
{
	public class BundleConfig
	{
		// For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
		public static void RegisterBundles(BundleCollection bundles)
		{
			//bundles.Add(new ScriptBundle("~/bundles/site").Include(
			//	"~/Scripts/Project/modal.js",
			//	"~/Scripts/Project/simplemodal.js",
			//	"~/Scripts/Project/datatables.extensions.js",
			//	"~/Scripts/Project/Cryptopia.js"));

			//bundles.Add(new ScriptBundle("~/bundles/dataexport").Include(
			//		"~/Scripts/jszip.js",
			//		"~/Scripts/pdfmake.js",
			//		"~/Scripts/vfs_fonts.js",
			//		"~/Scripts/dataTables.buttons.js",
			//		"~/Scripts/buttons.bootstrap.js",
			//		"~/Scripts/buttons.html5.js",
			//		"~/Scripts/buttons.print.js"));

			//bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
			//	"~/Scripts/jquery.validate.js",
			//	"~/Scripts/additional-methods.min.js",
			//	"~/Scripts/jquery.validate.unobtrusive.js"));

			bundles.Add(new StyleBundle("~/Content/css").Include(
				"~/Content/fontAwesome.css",
				"~/Content/bootstrap.css",
				"~/Content/whhg.css",
				"~/Content/Cryptopia.css"));

			//bundles.Add(new ScriptBundle("~/bundles/exchange").Include(
			//	"~/Scripts/Project/technical-indicators.src.js",
			//	"~/Scripts/Project/Exchange/exchange.js"));

			//bundles.Add(new ScriptBundle("~/bundles/userexchange").Include(
			//	"~/Scripts/Project/Exchange/user-exchange.js"));

			//bundles.Add(new ScriptBundle("~/bundles/exchangeinfo").Include(
			//	"~/Scripts/Project/Exchange/exchangeCharts.js",
			//	"~/Scripts/Project/Exchange/exchangeData.js"));


			//bundles.Add(new ScriptBundle("~/bundles/mineshaft").Include(
			//"~/Scripts/Project/Mineshaft/mineshaft.js"));

			//bundles.Add(new ScriptBundle("~/bundles/usermineshaft").Include(
			//	"~/Scripts/Project/Mineshaft/user-mineshaft.js"));

			//bundles.Add(new ScriptBundle("~/bundles/mineshaftinfo").Include(
			//	"~/Scripts/Project/Mineshaft/mineshaftCharts.js",
			//	"~/Scripts/Project/Mineshaft/mineshaftInfo.js"));
			
#if !DEBUG
			BundleTable.EnableOptimizations = true;
#endif
		}
	}
}