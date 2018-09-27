using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Web.Site.Helpers
{
	public static class CustomDropDownList
	{

		public static MvcHtmlString ExtendedDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
	Expression<Func<TModel, TProperty>> expression, IEnumerable<CustomSelectListItem> selectList,
	string optionLabel, object htmlAttributes)
		{
			if (expression == null)
				throw new ArgumentNullException(nameof(expression));
			var metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
			return htmlHelper.CustomDropDownListFor(metadata, optionLabel, ExpressionHelper.GetExpressionText(expression), selectList,
				HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
		}

		internal static MvcHtmlString CustomDropDownListFor(this HtmlHelper htmlHelper, ModelMetadata metadata, string optionLabel, string name,
			IEnumerable<CustomSelectListItem> selectList,
			IDictionary<string, object> htmlAttributes)
		{
			var fullName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
			if (string.IsNullOrEmpty(fullName))
				throw new ArgumentException("No name");

			if (selectList == null)
				throw new ArgumentException("No selectlist");

			var defaultValue = htmlHelper.GetModelStateValue(fullName, typeof(string));

			// If we haven't already used ViewData to get the entire list of items then we need to
			// use the ViewData-supplied value before using the parameter-supplied value.
			if (defaultValue == null)
				defaultValue = htmlHelper.ViewData.Eval(fullName);

			if (defaultValue != null)
			{
				var defaultValues = new[] { defaultValue };
				var values = from object value in defaultValues
										 select Convert.ToString(value, CultureInfo.CurrentCulture);
				var selectedValues = new HashSet<string>(values, StringComparer.OrdinalIgnoreCase);
				var newSelectList = new List<CustomSelectListItem>();

				foreach (var item in selectList)
				{
					item.Selected = (item.Value != null)
						? selectedValues.Contains(item.Value)
						: selectedValues.Contains(item.Text);
					newSelectList.Add(item);
				}
				selectList = newSelectList;
			}

			// Convert each ListItem to an <option> tag
			var listItemBuilder = new StringBuilder();

			// Make optionLabel the first item that gets rendered.
			if (optionLabel != null)
				listItemBuilder.Append(
					ListItemToOption(new CustomSelectListItem
					{
						Text = optionLabel,
						Value = string.Empty,
						Selected = false
					}));

			foreach (var item in selectList)
			{
				listItemBuilder.Append(ListItemToOption(item));
			}

			var tagBuilder = new TagBuilder("select")
			{
				InnerHtml = listItemBuilder.ToString()
			};
			tagBuilder.MergeAttributes(htmlAttributes);
			tagBuilder.MergeAttribute("name", fullName, true /* replaceExisting */);
			tagBuilder.GenerateId(fullName);

			// If there are any errors for a named field, we add the css attribute.
			ModelState modelState;
			if (htmlHelper.ViewData.ModelState.TryGetValue(fullName, out modelState))
			{
				if (modelState.Errors.Count > 0)
				{
					tagBuilder.AddCssClass(HtmlHelper.ValidationInputCssClassName);
				}
			}

			tagBuilder.MergeAttributes(htmlHelper.GetUnobtrusiveValidationAttributes(fullName, metadata));

			return MvcHtmlString.Create(tagBuilder.ToString(TagRenderMode.Normal));
		}

		internal static string ListItemToOption(CustomSelectListItem item)
		{
			var builder = new TagBuilder("option")
			{
				InnerHtml = HttpUtility.HtmlEncode(item.Text)
			};
			if (item.Value != null)
			{
				builder.Attributes["value"] = item.Value;
			}
			if (item.Selected)
			{
				builder.Attributes["selected"] = "selected";
			}
			builder.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(item.HtmlAttributes));
			return builder.ToString(TagRenderMode.Normal);
		}

		internal static object GetModelStateValue(this HtmlHelper htmlHelper, string key, Type destinationType)
		{
			ModelState modelState;
			if (htmlHelper.ViewData.ModelState.TryGetValue(key, out modelState))
			{
				if (modelState.Value != null)
				{
					return modelState.Value.ConvertTo(destinationType, null /* culture */);
				}
			}
			return null;
		}
	}

	public class CustomSelectListItem : SelectListItem
	{
		public CustomSelectListItem() { }
		public CustomSelectListItem(object value, string text, object htmlAttributes)
		{
			Text = text;
			Value = value.ToString();
			HtmlAttributes = htmlAttributes;
		}

		public object HtmlAttributes { get; set; }
	}
}
