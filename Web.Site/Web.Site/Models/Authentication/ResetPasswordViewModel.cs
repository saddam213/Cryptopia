using Cryptopia.Common.Validation;
using System.ComponentModel.DataAnnotations;

namespace Web.Site.Models
{
	public class ResetPasswordViewModel
    {
        [RequiredBase]
		[Display(Name = nameof(Resources.Authorization.loginEmailLabel), ResourceType = typeof(Resources.Authorization))]
		[EmailAddress(ErrorMessageResourceName = nameof(Resources.Authorization.loginEmailInvalidError), ErrorMessageResourceType = typeof(Resources.Authorization))]
		public string Email { get; set; }

        [RequiredBase]
        [StringLength(100, MinimumLength = 6, ErrorMessageResourceName = nameof(Resources.Authorization.resetPasswordRangeError), ErrorMessageResourceType = typeof(Resources.Authorization))]
        [DataType(DataType.Password)]
		[Display(Name = nameof(Resources.Authorization.resetPasswordLabel), ResourceType = typeof(Resources.Authorization))]
		public string Password { get; set; }

        [DataType(DataType.Password)]
		[Display(Name = nameof(Resources.Authorization.resetPassConfirmLabel), ResourceType = typeof(Resources.Authorization))]
		[Compare("Password", ErrorMessageResourceName = nameof(Resources.Authorization.resetPasswordConfirmError), ErrorMessageResourceType = typeof(Resources.Authorization))]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }
}