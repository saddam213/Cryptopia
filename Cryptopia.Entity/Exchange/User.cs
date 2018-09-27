using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Cryptopia.Enums;

namespace Cryptopia.Entity
{
	public class User
	{
		[Key]
		public Guid Id { get; set; }

		[MaxLength(256)]
		public string UserName { get; set; }

		[MaxLength(256)]
		public string Email { get; set; }

		[MaxLength(256)]
		public string ChatHandle { get; set; }

		[MaxLength(256)]
		public string MiningHandle { get; set; }

		[MaxLength(256)]
		public string Referrer { get; set; }

		public double TrustRating { get; set; }

		public bool DisableRewards { get; set; }

		public bool DisableTipNotify { get; set; }
			
		public bool DisablePoolNotify { get; set; }

		public bool DisableExchangeNotify { get; set; }

		public bool DisableFaucetNotify { get; set; }

		public bool DisableMarketplaceNotify { get; set; }

		public bool IsDisabled { get; set; }

		public DateTime? RegisterDate { get; set; }

		public bool DisableWithdrawEmailConfirmation { get; set; }
		public bool IsUnsafeWithdrawEnabled { get; set; }
		public bool IsApiUnsafeWithdrawEnabled { get; set; }
		public VerificationLevel VerificationLevel { get; set; }

		public virtual ICollection<LottoTicket> LottoTickets { get; set; }
	}
}