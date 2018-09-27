using Cryptopia.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptopia.Admin.Common.Nzdt
{
	public class UpdateNzdtTransactionModel
	{
		[Required]
		public int TransactionId { get; set; }

		[Required]
		public NzdtTransactionStatus Status { get; set; }

		[Required]
		public string UserName { get; set; }

		public DateTime CreatedOn { get; set; }
		public DateTime Date { get; set; }
		public long UniqueId { get; set; }
		public string Memo { get; set; }
		public decimal Amount { get; set; }

		public string VerificationLevel { get; set; }
	}
}
