using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cryptopia.Entity
{
	public class TermDepositItem
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		[MaxLength(256)]
		public string Name { get; set; }

		[MaxLength(4000)]
		public string Description { get; set; }

		[MaxLength(256)]
		public string Address { get; set; }

		[MaxLength(256)]
		public string PrivateKey { get; set; }
		public decimal InterestRate { get; set; }
		public decimal MinInvestment { get; set; }
	
		public bool IsEnabled { get; set; }
		public int TermLength { get; set; }

		public virtual ICollection<TermDeposit> TermDeposits { get; set; }
		
	}
}
