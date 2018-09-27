using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptopia.Admin.Common.Fiat
{
	public class DepositFiatModel
	{
		public string Reference { get; set; }
		public decimal Amount { get; set; }

		public string TwoFactorCode { get; set; }
	}
}
