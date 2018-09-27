using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptopia.Common.Trade
{
	public class CreateWithdrawResponseModel
	{
		public string Error { get; set; }
		public bool IsError
		{
			get { return !string.IsNullOrEmpty(Error); }
		}

		public int WithdrawId { get; set; }
	}
}
