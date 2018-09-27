using System.ComponentModel.DataAnnotations;

namespace Cryptopia.Admin.Common.AdminUser
{
	public class AdminChangeEmailModel
	{
		public string UserName { get; set; }
		public string EmailAddress { get; set; }

		[EmailAddress]
		public string NewEmailAddress { get; set; }
	}
}