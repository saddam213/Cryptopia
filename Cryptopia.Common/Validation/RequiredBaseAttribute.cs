using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptopia.Common.Validation
{
	/// <summary>
	/// RequiredAttribute localization (use Resources.Validation.attributeRequiredError error message)
	/// </summary>
	public class RequiredBaseAttribute : RequiredAttribute
	{
		/// <summary>
		/// Use default localized error message if custom message is not set via attribute parameters
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public override string FormatErrorMessage(string name)
		{
			if (string.IsNullOrEmpty(ErrorMessage) && ErrorMessageResourceType == null && String.IsNullOrEmpty(ErrorMessageResourceName))
				return String.Format(Resources.Validation.attributeRequiredError, name);
			else
				return base.FormatErrorMessage(name); 
		}
	}	
}
