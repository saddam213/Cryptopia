using Cryptopia.Common.Validation;
using System.ComponentModel.DataAnnotations;

namespace Web.Site.Models
{
	public class ForgotPasswordViewModel
    {
		[RequiredBase]
		[Display(Name = nameof(Resources.Authorization.loginEmailLabel), ResourceType = typeof(Resources.Authorization))]
		[EmailAddress(ErrorMessageResourceName = nameof(Resources.Authorization.loginEmailInvalidError), ErrorMessageResourceType = typeof(Resources.Authorization))]
		public string Email { get; set; }
    }

}