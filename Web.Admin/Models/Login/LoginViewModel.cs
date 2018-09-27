using System.ComponentModel.DataAnnotations;

namespace Web.Admin.Models
{
	public class LoginViewModel
	{
		[Required]
		[Display(Name = "Email Address")]
		[EmailAddress]
		public string EmailAddress { get; set; }

		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "Password")]
		[System.Web.Mvc.AllowHtml]
		public string Password { get; set; }

		[Required]
		public string TwoFactor { get; set; }
	}
}