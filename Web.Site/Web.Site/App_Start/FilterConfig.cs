using System.Web.Mvc;
using Web.Site.ActionFilters;

namespace Web.Site
{
	public class FilterConfig
	{
		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
			//filters.Add(new WhitespaceFilter(), 1);
			//filters.Add(new ETagFilterAttribute(), 2);
		}
	}
}
