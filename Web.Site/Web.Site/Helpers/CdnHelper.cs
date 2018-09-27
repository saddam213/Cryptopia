using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Optimization;

namespace Web.Site.Helpers
{
	public static class CdnHelper
	{
		private static readonly bool _imageCdnEnabled = bool.Parse(WebConfigurationManager.AppSettings["CDN_Image_Enabled"] ?? "false");
		private static readonly bool _scriptCdnEnabled = bool.Parse(WebConfigurationManager.AppSettings["CDN_Script_Enabled"] ?? "false");
		private static readonly bool _styleCdnEnabled = bool.Parse(WebConfigurationManager.AppSettings["CDN_Style_Enabled"] ?? "false");
		private static readonly string _imageCdnPath = WebConfigurationManager.AppSettings["CDN_Image_Endpoint"].TrimEnd('/');
		private static readonly string _scriptCdnPath = WebConfigurationManager.AppSettings["CDN_Script_Endpoint"].TrimEnd('/');
		private static readonly string _styleCdnPath = WebConfigurationManager.AppSettings["CDN_Style_Endpoint"].TrimEnd('/');

		public static IHtmlString RenderScript(string path)
		{
			if (_scriptCdnEnabled && !string.IsNullOrEmpty(_scriptCdnPath))
			{
				return MvcHtmlString.Create($"<script src=\"{_scriptCdnPath}{path.TrimStart('~')}\"></script>");
			}
			return Scripts.Render(path);
		}

		public static IHtmlString RenderStyle(string path, string title = null)
		{
			if (_styleCdnEnabled && !string.IsNullOrEmpty(_styleCdnPath))
			{
				return string.IsNullOrEmpty(title) 
				? MvcHtmlString.Create($"<link href=\"{_styleCdnPath}{path.TrimStart('~')}\" rel=\"stylesheet\" />")
				: MvcHtmlString.Create($"<link title=\"{title}\" href=\"{_styleCdnPath}{path.TrimStart('~')}\" rel=\"stylesheet\" />");
			}
			return string.IsNullOrEmpty(title)
				? Styles.Render(path)
				: MvcHtmlString.Create($"<link title=\"{title}\" href=\"{path.TrimStart('~')}\" rel=\"stylesheet\" />");
		}

		public static IHtmlString RenderImage(string path)
		{
			if (_imageCdnEnabled && !string.IsNullOrEmpty(_imageCdnPath))
			{
				return MvcHtmlString.Create($"{_imageCdnPath}{path.TrimStart('~')}");
			}
			return MvcHtmlString.Create(path.TrimStart('~'));
		}

		public static IHtmlString RenderImage(string pathFormat, params object[] param)
		{
			return RenderImage(string.Format(pathFormat, param));
		}

		public static IHtmlString ImagePath()
		{
			if (_imageCdnEnabled && !string.IsNullOrEmpty(_imageCdnPath))
			{
				return MvcHtmlString.Create(_imageCdnPath);
			}
			return MvcHtmlString.Empty;
		}
	}
}
