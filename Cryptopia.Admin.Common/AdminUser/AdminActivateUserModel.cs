using System.ComponentModel.DataAnnotations;

namespace Cryptopia.Admin.Common.AdminUser
{
	public class AdminActivateUserModel
	{
		public string UserName { get; set; }
		public string EmailAddress { get; set; }
		public bool IsActivated { get; set; }
	}
}