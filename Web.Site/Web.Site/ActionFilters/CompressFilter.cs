using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Web.Site.ActionFilters
{
	public sealed class CompressFilter : ActionFilterAttribute
	{
		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			bool allowCompression = true;
		//	bool.TryParse(System.Configuration.ConfigurationManager.AppSettings["Compression"], out allowCompression);

			if (allowCompression)
			{
				HttpRequestBase request = filterContext.HttpContext.Request;

				string acceptEncoding = request.Headers["Accept-Encoding"];

				if (string.IsNullOrEmpty(acceptEncoding)) return;

				acceptEncoding = acceptEncoding.ToUpperInvariant();

				HttpResponseBase response = filterContext.HttpContext.Response;

				if (acceptEncoding.Contains("GZIP"))
				{
					response.AppendHeader("Content-encoding", "gzip");
					response.Filter = new GZipStream(response.Filter, CompressionMode.Compress);
				}
				else if (acceptEncoding.Contains("DEFLATE"))
				{
					response.AppendHeader("Content-encoding", "deflate");
					response.Filter = new DeflateStream(response.Filter, CompressionMode.Compress);
				}
			}
		}
	}

}
