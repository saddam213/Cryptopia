using System.Web;
using System.Web.Mvc;

namespace Web.Site.Extensions
{
	public sealed class AuthorizeAjaxAttribute : AuthorizeAttribute
	{
		protected override void HandleUnauthorizedRequest(AuthorizationContext context)
		{
			if (context.HttpContext.Request.IsAjaxRequest())
			{
				context.HttpContext.Response.StatusCode = 403;
				context.Result = new JsonResult
				{
					Data = new
					{
						Error = "NotAuthorized",
						LogOnUrl = $"/Login?ReturnUrl={HttpUtility.UrlEncode(context.HttpContext.Request.UrlReferrer?.LocalPath)}"
					},
					JsonRequestBehavior = JsonRequestBehavior.AllowGet,
				};
			}
			else
			{
				base.HandleUnauthorizedRequest(context);
			}
		}
	}
}
