using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using Cryptopia.Enums;
using Cryptopia.Entity.Auditing;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cryptopia.Entity
{
	public class ApplicationUser : IdentityUser, IAuditable
	{
        [Auditable(false, false)]
        public string ChatHandle { get; set; }
		public string MiningHandle { get; set; }
		public double TrustRating { get; set; }
		public string Referrer { get; set; }
		public string RoleCss { get; set; }
		public DateTime? ChatBanEndTime { get; set; }
		public DateTime? ChatTipBanEndTime { get; set; }
		public bool ChatDisableEmoticons { get; set; }
        [Auditable(false, false)]
        public bool DisableLogonEmail { get; set; }
		public bool DisableTips { get; set; }
		public bool DisableTipNotify { get; set; }
		public bool DisablePoolNotify { get; set; }
		public bool DisableExchangeNotify { get; set; }
		public bool DisableFaucetNotify { get; set; }
		public bool DisableMarketplaceNotify { get; set; }
		public DateTime? RegisterDate { get; set; }

		[MaxLength(4000)]
		public string ChatIgnoreList { get; set; }

		[MaxLength(4000)]
		public string ChatTipIgnoreList { get; set; }

		[MaxLength(1000)]
		public string ForumSignature { get; set; }

		public int KarmaTotal { get; set; }

		public bool IsDisabled { get; set; }

		[MaxLength(128)]
        [Auditable(false, false)]
        public string ApiKey { get; set; }

		[MaxLength(256)]
        public string ApiSecret { get; set; }
        [Auditable(false, false)]
        public bool IsApiEnabled { get; set; }
        [Auditable(false, false)]
        public bool IsApiWithdrawEnabled { get; set; }
        [Auditable(false, false)]
        public bool IsApiUnsafeWithdrawEnabled { get; set; }
        [Auditable(false, false)]
        public bool DisableWithdrawEmailConfirmation { get; set; }

		public bool DisableRewards { get; set; }
		public bool DisableKarmaNotify { get; set; }

		public virtual ICollection<UserTwoFactor> TwoFactor { get; set; }
		public virtual ICollection<ChatMessage> ChatMessages { get; set; }
		public virtual ICollection<UserNotification> Notifications { get; set; }
		public virtual ICollection<UserMessage> Messages { get; set; }
		public virtual ICollection<UserLogon> Logons { get; set; }
		public virtual UserSettings Settings { get; set; }
		public virtual UserProfile Profile { get; set; }
		public virtual ICollection<ForumThread> ForumThreads { get; set; }
		public virtual ICollection<ForumPost> ForumPosts { get; set; }
		public int ShareCount { get; set; }
		public string ChartSettings { get; set; }
        [Auditable(false, false)]
		public bool IsUnsafeWithdrawEnabled { get; set; }
		public DateTime SettingsUnlocked { get; set; }
		public VerificationLevel VerificationLevel { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
		{
			var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
			userIdentity.AddClaim(new Claim(CryptopiaClaim.Theme, this.Settings.Theme.ToString()));
			userIdentity.AddClaim(new Claim(CryptopiaClaim.ChatHandle, this.ChatHandle));
			userIdentity.AddClaim(new Claim(CryptopiaClaim.HideZeroBalance, this.Settings.HideZeroBalance.ToString()));
			userIdentity.AddClaim(new Claim(CryptopiaClaim.ShowFavoriteBalance, this.Settings.ShowFavoriteBalance.ToString()));
			userIdentity.AddClaim(new Claim(CryptopiaClaim.Shareholder, this.ShareCount.ToString()));
			userIdentity.AddClaim(new Claim(CryptopiaClaim.ChartSettings, this.ChartSettings ?? string.Empty));
			return userIdentity;
		}
	}
}