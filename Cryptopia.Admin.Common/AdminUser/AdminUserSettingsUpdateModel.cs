using Cryptopia.Enums;

namespace Cryptopia.Admin.Common.AdminUser
{
	public class AdminUserSettingsUpdateModel
	{
		public string UserName { get; set; }
		public SiteTheme Theme { get; set; }
		public bool ShowFavoriteBalance { get; set; }
		public bool HideZeroBalance { get; set; }
		public string Id { get; set; }
		public bool ChatDisableEmoticons { get; set; }
		public bool DisableLogonEmail { get; set; }
		public bool DisableRewards { get; set; }
		public bool DisableTips { get; set; }
	}
}