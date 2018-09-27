using System.Web.Mvc;
using Cryptopia.Infrastructure.Common.Results;

namespace Web.Site.Helpers
{
	public static class ModelStateHelpers
	{
		public static bool IsWriterResultValid(this ModelStateDictionary modelState, IWriterResult writerResult)
		{
			bool valid = modelState.IsValid && writerResult.Success;
			if (!string.IsNullOrEmpty(writerResult.Message))
			{
				modelState.AddModelError(writerResult.Success ? "Success" : "Error", writerResult.Message);
			}
			return valid;
		}
	}
}
