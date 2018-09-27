namespace Cryptopia.Admin.Common.AdminUser
{
	public class AdminUserApiUpdateModel
	{
		public string UserName { get; set; }
		public bool IsApiEnabled { get; set; }
		public bool IsApiUnsafeWithdrawEnabled { get; set; }
		public bool IsApiWithdrawEnabled { get; set; }
		public string Id { get; set; }
	}
}