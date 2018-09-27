using System.Web;
using System.Web.Mvc;

namespace Web.Site
{
	public static class HtmlHelpers
    {
        //public static string ExternalLink(this HtmlHelper helper, string uri)
        //{
        //    return string.Format(@"<u><a href='{0}'>{1}</a></u>", (string.IsNullOrEmpty(uri) || uri == "N/A") ? "#" : uri, uri);
        //}

        public static IHtmlString ExternalLink(this HtmlHelper htmlHelper, string uri)
        {
            return MvcHtmlString.Create(string.Format(@"<u><a href='{0}'>{1}</a></u>", (string.IsNullOrEmpty(uri) || uri == "N/A") ? "#" : uri, uri));
        }

		public static bool IsDebug(this HtmlHelper htmlHelper)
		{
#if DEBUG
      return true;
#else
			return false;
#endif
		}

	}
}