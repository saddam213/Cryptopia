using Cryptopia.Common.Validation;
using System.ComponentModel.DataAnnotations;


namespace Web.Site.Models
{
	public class RegisterViewModel
	{

		[RequiredBase]
		[Display(Name = nameof(Resources.Authorization.registerUserNameLabel), ResourceType = typeof(Resources.Authorization))]
		[RegularExpression(@"^\w+$", ErrorMessageResourceName = nameof(Resources.Authorization.registerUserNameCharError), ErrorMessageResourceType = typeof(Resources.Authorization))]
		public string UserName { get; set; }

		[RequiredBase]
		[Display(Name = nameof(Resources.Authorization.loginEmailLabel), ResourceType = typeof(Resources.Authorization))]
		[EmailAddress(ErrorMessageResourceName = nameof(Resources.Authorization.loginEmailInvalidError), ErrorMessageResourceType = typeof(Resources.Authorization))]
		public string EmailAddress { get; set; }

		[System.Web.Mvc.AllowHtml]
		[RequiredBase]
		[StringLength(100, MinimumLength = 8, ErrorMessageResourceName = nameof(Resources.Authorization.registerPasswordLengthRangeError), ErrorMessageResourceType = typeof(Resources.Authorization))]
		[DataType(DataType.Password)]
		[Display(Name = nameof(Resources.Authorization.registerPasswordLabel), ResourceType = typeof(Resources.Authorization))]
		public string Password { get; set; }

		[System.Web.Mvc.AllowHtml]
		[DataType(DataType.Password)]
		[Display(Name = nameof(Resources.Authorization.registerPassConfirmLabel), ResourceType = typeof(Resources.Authorization))]
		[Compare("Password", ErrorMessageResourceName = nameof(Resources.Authorization.registerPassConfirmError), ErrorMessageResourceType = typeof(Resources.Authorization))]
		public string ConfirmPassword { get; set; }

		[RequiredBase]
		[Display(Name = nameof(Resources.Authorization.registerPinCodeLabel), ResourceType = typeof(Resources.Authorization))]
		[RegularExpression("^[0-9]{4,8}$", ErrorMessageResourceName = nameof(Resources.Authorization.registerPinCodeLengthError), ErrorMessageResourceType = typeof(Resources.Authorization))]
		public string PinCode { get; set; }

		[RequiredToBeTrue(ErrorMessageResourceName = nameof(Resources.Authorization.registerAcceptError), ErrorMessageResourceType = typeof(Resources.Authorization))]
		public bool AcceptTerms { get; set; }

		public string Referrer { get; set; }
	}
}