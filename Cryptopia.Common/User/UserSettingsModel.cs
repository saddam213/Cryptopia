using System.Collections.Generic;
using Cryptopia.Enums;

namespace Cryptopia.Common.User
{
	public class UserSettingsModel
	{
		public UserSettingsModel()
		{
			IgnoreChatList = new List<string>();
			IgnoreTipList = new List<string>();
		}

		public bool DisableLogonEmail { get; set; }
		public bool DisableRewards { get; set; }
		public bool DisableTips { get; set; }
		public bool DisableTipNotify { get; set; }
		public bool DisableKarmaNotify { get; set; }
		public bool DisablePoolNotify { get; set; }
		public bool DisableExchangeNotify { get; set; }
		public bool DisableFaucetNotify { get; set; }
		public bool DisableMarketplaceNotify { get; set; }
		public bool DisableChatEmoticons { get; set; }
		public List<string> IgnoreTipList { get; set; }
		public List<string> IgnoreChatList { get; set; }
		public SiteTheme Theme { get; set; }
	}
}