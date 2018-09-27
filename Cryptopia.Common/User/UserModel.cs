using System;
using System.Collections.Generic;
using Cryptopia.Enums;

namespace Cryptopia.Common.User
{
	public class UserModel
	{
		public string UserId { get; set; }
		public string UserName { get; set; }
		public string RoleCss { get; set; }
		public DateTime? ChatBanEndTime { get; set; }
		public DateTime? ChatTipBanEndTime { get; set; }
		public double TrustRating { get; set; }
		public int ShareCount { get; set; }
		public int KarmaTotal { get; set; }
		public bool IsApiEnabled { get; set; }
		public bool IsApiUnsafeWithdrawEnabled { get; set; }
		public bool IsApiWithdrawEnabled { get; set; }
		public bool EmailConfirmed { get; set; }
		public bool DisableLogonEmail { get; set; }
		public bool DisableRewards { get; set; }
		public bool DisableTips { get; set; }
		public DateTime? LockoutEnd { get; set; }
		public string MiningHandle { get; set; }
		public string ChatHandle { get; set; }
		public List<string> TwoFactor { get; set; }
		public string Email { get; set; }
		public string Referrer { get; set; }
		public SiteTheme Theme { get; set; }
		public VerificationLevel VerificationLevel { get; set; }
	}
}