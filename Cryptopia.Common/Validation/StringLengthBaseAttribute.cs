using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptopia.Common.Validation
{
    public class StringLengthBaseAttribute : StringLengthAttribute
	{
		public StringLengthBaseAttribute(int maximumLength)
			: base(maximumLength)
		{
		}

		public override string FormatErrorMessage(string name)
		{
			if (string.IsNullOrEmpty(ErrorMessage) && ErrorMessageResourceType == null && String.IsNullOrEmpty(ErrorMessageResourceName))
			{
				if (MinimumLength > 0)
					return String.Format(Resources.Validation.attributeStringLengthRangeError, name, MaximumLength, MinimumLength);
				else
					return String.Format(Resources.Validation.attributeStringLengthMaxError, name, MaximumLength);
			}
			else
				return base.FormatErrorMessage(name);
		}
	}
}
