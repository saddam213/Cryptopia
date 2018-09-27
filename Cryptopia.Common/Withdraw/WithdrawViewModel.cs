using Cryptopia.Common.Currency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptopia.Common.Withdraw
{
	public class WithdrawViewModel
	{
		public string Symbol { get; set; }
		public List<CurrencyModel> Currencies { get; set; }
		public string ReturnUrl { get; set; }
	}
}
