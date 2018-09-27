using System;
using System.ComponentModel.DataAnnotations;

namespace Cryptopia.Common.Validation
{
	public class RequiredIfAttribute : RequiredBaseAttribute
	{
		private String PropertyName { get; set; }
		private Object DesiredValue { get; set; }

		public RequiredIfAttribute(String propertyName, Object desiredvalue)
		{
			PropertyName = propertyName;
			DesiredValue = desiredvalue;
		}

		protected override ValidationResult IsValid(object value, ValidationContext context)
		{
			Object instance = context.ObjectInstance;
			Type type = instance.GetType();
			Object proprtyvalue = type.GetProperty(PropertyName).GetValue(instance, null);
			if (proprtyvalue.ToString() == DesiredValue.ToString())
			{
				ValidationResult result = base.IsValid(value, context);
				return result;
			}
			return ValidationResult.Success;
		}
	}
}
