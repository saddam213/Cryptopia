using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cryptopia.Enums;

namespace Cryptopia.Common.Withdraw
{
	public class WithdrawModel
	{
		public string Address { get; set; }
		public decimal Amount { get; set; }
		public AddressType AddressType { get; set; }
		public string Currency { get; set; }
		public bool DisableWithdrawEmailConfirmation { get; set; }
		public decimal Fee { get; set; }
		public int Id { get; set; }
		public string Email { get; set; }

		public string ReturnUrl { get; set; }
	}
}
