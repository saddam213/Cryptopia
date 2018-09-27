using System.Linq;
using System.Web.Mvc;

namespace Web.Admin.Extensions
{
	public static class ValidationExtensions
	{
		public static string FlattenErrors(this ModelStateDictionary modelState)
		{
			if (modelState != null && modelState.Any())
			{
				return string.Join("<br />", modelState.Where(x => x.Value.Errors.Count != 0)
												.SelectMany(x => x.Value.Errors)
												.Select(x => x.ErrorMessage)
												.ToArray());
			}
			return string.Empty;
		}

		public static string FirstError(this ModelStateDictionary modelState)
		{
			if (modelState != null && modelState.Any())
			{
				var error = modelState.Where(x => x.Value.Errors.Count != 0).SelectMany(x => x.Value.Errors).FirstOrDefault();
				return error != null ? error.ErrorMessage : "An unknown error occured, if problem persists please contact support.";
			}
			return string.Empty;
		}
	}
}