using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptopia.Common.TermDeposits
{
	public class CreateTermDepositModel
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public decimal Minimum { get; set; }
		public decimal InterestRate { get; set; }
		public int TermLength { get; set; }
	
		public decimal Balance { get; set; }

		[Range(1, 100)]
		public int Count { get; set; }
	//	public int TermDepositId { get; set; }
	}
}
