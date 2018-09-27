using System.Security.Claims;
using System.Web.Mvc;
using Web.Site.Helpers;

namespace Web.Site.Views
{
	public abstract class ClaimsViewPage<TModel> : WebViewPage<TModel>
	{
		protected ClaimsUser ClaimsUser
		{
			get { return new ClaimsUser(User as ClaimsPrincipal); }
		}

		protected MvcHtmlString GetReturnUrl()
		{
			var path = Context.Request.Url.LocalPath;
			var homeAction = Url.Action("Index", "Home");
			if (Url.IsLocalUrl(path))
			{
				if (path == "/" || path == homeAction)
					return MvcHtmlString.Empty;

				return MvcHtmlString.Create($"?returnUrl={path}");
			}
			return MvcHtmlString.Empty;
		}
	}

	public abstract class ClaimsViewPage : ClaimsViewPage<dynamic>
	{
	}
}
