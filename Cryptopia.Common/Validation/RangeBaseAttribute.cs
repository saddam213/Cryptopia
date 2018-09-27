using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptopia.Common.Validation
{
	public class RangeBaseAttribute : RangeAttribute
	{
		public RangeBaseAttribute(int minimum, int maximum): base(minimum, maximum){ }
		public RangeBaseAttribute(double minimum, double maximum) : base(minimum, maximum) { }
		public RangeBaseAttribute(Type type, string minimum, string maximum) : base(type, minimum, maximum){ }

		/// <summary>
		/// Use default localized error message if custom message is not set via attribute parameters
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public override string FormatErrorMessage(string name)
		{
			if (string.IsNullOrEmpty(ErrorMessage) && ErrorMessageResourceType == null && String.IsNullOrEmpty(ErrorMessageResourceName))
				return String.Format(Resources.Validation.attributeRangeError, name, Minimum, Maximum);
			else
				return base.FormatErrorMessage(name);
		}
	}
}
