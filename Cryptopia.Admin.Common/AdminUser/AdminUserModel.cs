using System;
using System.Collections.Generic;

namespace Cryptopia.Admin.Common.AdminUser
{
	public class AdminUserModel
	{
		public string UserName { get; set; }
		public string Referrer { get; set; }
		public string ChatHandle { get; set; }
		public string MiningHandle { get; set; }
		public DateTime? LockoutEnd { get; set; }
		public bool DisableTips { get; set; }
		public bool DisableRewards { get; set; }
		public bool EmailConfirmed { get; set; }
		public int KarmaTotal { get; set; }
		public int ShareCount { get; set; }
		public string RoleCss { get; set; }
		public double TrustRating { get; set; }
		public string Email { get; set; }
		public DateTime? RegisterDate { get; set; }
		public bool IsDisabled { get; set; }
		public bool ChatDisableEmoticons { get; set; }
		public string VerificationLevel { get; set; }
	}

	public class AdminUserDetailsModel
	{
		public string UserName { get; set; }
		public string Referrer { get; set; }
		public string ChatHandle { get; set; }
		public string MiningHandle { get; set; }
		public DateTime? LockoutEnd { get; set; }
		public bool DisableTips { get; set; }
		public bool DisableRewards { get; set; }
		public bool EmailConfirmed { get; set; }
		public int KarmaTotal { get; set; }
		public int ShareCount { get; set; }
		public string RoleCss { get; set; }
		public double TrustRating { get; set; }
		public string Email { get; set; }
		public bool IsLocked { get; set; }
		public DateTime? RegisterDate { get; set; }
		public bool IsDisabled { get; set; }
		public bool ChatDisableEmoticons { get; set; }
		public string Theme { get; set; }
		public string VerificationLevel { get; set; }


		public string FirstName { get; set; }
		public string LastName { get; set; }
		public DateTime Birthday { get; set; }
		public string Gender { get; set; }
		public string Address { get; set; }
		public string City { get; set; }
		public string State { get; set; }
		public string Postcode { get; set; }
		public string Country { get; set; }
		public string ContactEmail { get; set; }
		public string Occupation { get; set; }
		public string Education { get; set; }
		public string Facebook { get; set; }
		public string Hobbies { get; set; }
		public string AboutMe { get; set; }
		public string LinkedIn { get; set; }
		public string Twitter { get; set; }
		public string Website { get; set; }
		public bool IsPublic { get; set; }
	}

	public class AdminUserTwoFactorItemModel
	{
		public Enums.TwoFactorComponent Component { get; set; }
		public string Type { get; set; }
		public object Name { get; set; }
	}

	public class AdminUserSecurityModel
	{
		public string UserName { get; set; }
		public bool DisableLogonEmail { get; set; }
		public bool IsApiEnabled { get; set; }
		public bool IsApiWithdrawEnabled { get; set; }
		public bool IsApiUnsafeWithdrawEnabled { get; set; }
		public bool DisableWithdrawEmailConfirmation { get; set; }
		public bool IsUnsafeWithdrawEnabled { get; set; }
		public List<AdminUserTwoFactorItemModel> TwoFactor { get; set; }
	}
}
