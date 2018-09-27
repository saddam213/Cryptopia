using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Web.Site.Helpers
{
	public static class HtmlHelpers
	{
		public static MvcHtmlString WriterValidationSummary<TModel>(this HtmlHelper<TModel> helper)
		{
			var name = helper.ViewData.Model == null ? "writerAlert"
				: string.Format("writerAlert-{0}", helper.ViewData.Model.GetType().Name);
			var type = helper.ViewData.ModelState.Any(x => x.Key == "Error") ? "danger" : "success";
			if (helper.ViewData.ModelState.Any(x => x.Key == "Error"))
			{
				var error = helper.ViewData.ModelState.FirstOrDefault(x => x.Key == "Error").Value.Errors.FirstOrDefault();
				if (error != null)
					return MvcHtmlString.Create(BuildAlert("danger", name, error.ErrorMessage));
			}
			else if (helper.ViewData.ModelState.Any(x => x.Key == "Success"))
			{
				var message = helper.ViewData.ModelState.FirstOrDefault(x => x.Key == "Success").Value.Errors.FirstOrDefault();
				if (message != null)
					return MvcHtmlString.Create(BuildAlert("success", name, message.ErrorMessage));
			}

			return MvcHtmlString.Empty;
		}

		public static MvcHtmlString ModelValidationSummary<TModel>(this HtmlHelper<TModel> helper)
		{
			if (helper.ViewData.ModelState.Any(x => x.Key == "" || x.Key == "Error"))
			{
				var error = helper.ViewData.ModelState.FirstOrDefault(x => x.Key == "" || x.Key == "Error").Value.Errors.FirstOrDefault();
				if (error != null)
					return MvcHtmlString.Create(BuildErrorAlert(error.ErrorMessage));
			}
			return MvcHtmlString.Empty;
		}


		private static string BuildAlert(string type, string name, string message)
		{
			var stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(string.Format("<div class='{1} alert alert-{0} text-center' style='display:none'>", type, name));
			stringBuilder.AppendLine(string.Format("<p>{0}</p>", message));
			stringBuilder.AppendLine("<script>");
			stringBuilder.AppendLine("$(function(){ $('." + name + "').fadeTo(8000, 500).slideUp(500, function () {$('." + name + "').hide();	}); });");
			stringBuilder.AppendLine("</script>");
			stringBuilder.AppendLine("</div>");
			return stringBuilder.ToString();
		}

		private static string BuildErrorAlert(string message)
		{
			var stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("<div class='alert alert-danger text-center'>");
			stringBuilder.AppendLine(string.Format("<p>{0}</p>", message));
			stringBuilder.AppendLine("</div>");
			return stringBuilder.ToString();
		}

		public static MvcHtmlString CurrentThreadCulture(this HtmlHelper helper)
		{
			return MvcHtmlString.Create(Localization.CurrentCulture);
		}
	}
}
