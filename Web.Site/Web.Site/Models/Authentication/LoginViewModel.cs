using Cryptopia.Common.Validation;
using System.ComponentModel.DataAnnotations;
using Web.Site.Helpers;

namespace Web.Site.Models
{
	public class LoginViewModel
	{
		[RequiredBase]
		[Display(Name = nameof(Resources.Authorization.loginEmailLabel), ResourceType = typeof(Resources.Authorization))]
		[EmailAddress(ErrorMessageResourceName = nameof(Resources.Authorization.loginEmailInvalidError), ErrorMessageResourceType = typeof(Resources.Authorization))]
		public string EmailAddress { get; set; }

		[RequiredBase]
		[DataType(DataType.Password)]
		[Display(Name = nameof(Resources.Authorization.loginPasswordLabel), ResourceType = typeof(Resources.Authorization))]
		[System.Web.Mvc.AllowHtml]
		public string Password { get; set; }
	}
}