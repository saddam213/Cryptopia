using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptopia.Admin.Common.Fiat
{
	public class WithdrawFiatModel
	{
		public string UserName { get; set; }
		public string Reference { get; set; }
		public decimal Amount { get; set; }

		public string TwoFactorCode { get; set; }
	}
}
