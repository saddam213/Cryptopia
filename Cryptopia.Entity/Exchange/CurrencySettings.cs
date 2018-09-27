using System;
using System.ComponentModel.DataAnnotations;
using Cryptopia.Enums;

namespace Cryptopia.Entity
{
	public class CurrencySettings
	{
		[Key]
		public int Id { get; set; }


		[MaxLength(128)]
		public string QrFormat { get; set; }

		[MaxLength(4000)]
		public string DepositInstructions { get; set; }

		[MaxLength(4000)]
		public string DepositMessage { get; set; }

		public AlertType DepositMessageType { get; set; }

		[MaxLength(4000)]
		public string WithdrawInstructions { get; set; }

		[MaxLength(4000)]
		public string WithdrawMessage { get; set; }

		public int Decimals { get; set; }

		public AlertType WithdrawMessageType { get; set; }

		public AddressType AddressType { get; set; }

		public DateTime? DelistOn { get; set; }

		public virtual Currency Currency { get; set; }
	
	}
}