using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Cryptopia.Common.Validation
{
	public class RequiredToBeTrueAttribute : RequiredBaseAttribute
	{
		public override bool IsValid(object value)
		{
			if (value is bool)
				return (bool)value;
			else
				return true;
		}

		public IEnumerable<ModelClientValidationRule> GetClientValidationRules(
				ModelMetadata metadata,
				ControllerContext context)
		{
			yield return new ModelClientValidationRule
			{
				ErrorMessage = FormatErrorMessage(metadata.GetDisplayName()),
				ValidationType = "booleanrequired"
			};
		}

	}
}
