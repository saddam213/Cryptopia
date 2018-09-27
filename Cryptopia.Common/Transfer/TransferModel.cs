using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptopia.Common.Transfer
{
	public class TransferModel
	{
		public int Id { get; set; }
		public decimal Amount { get; set; }
		public string Name { get; set; }
		public string Symbol { get; set; }
		public string Receiver { get; set; }
		public string ReturnUrl { get; set; }
	}
}
