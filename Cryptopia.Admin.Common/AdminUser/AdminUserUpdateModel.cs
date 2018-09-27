namespace Cryptopia.Admin.Common.AdminUser
{
	public class AdminUserUpdateModel
	{
		public string UserName { get; set; }
		public bool EmailConfirmed { get; set; }
		public string RoleCss { get; set; }
		public int ShareCount { get; set; }
		public string Id { get; set; }
		public string UserId { get; set; }
	}
}