using Cryptopia.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptopia.Entity
{
	public class PaytopiaPayment
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }
		public int PaytopiaItemId { get; set; }

		[MaxLength(128)]
		public string UserId { get; set; }
		public PaytopiaPaymentStatus Status { get; set; }
		public decimal Amount { get; set; }
		public DateTime Begins { get; set; }
		public DateTime Ends { get; set; }
		public bool IsAnonymous { get; set; }
		public int TransferId { get; set; }
		public int RefundId { get; set; }
		public int ReferenceId { get; set; }

		[MaxLength(128)]
		public string ReferenceCode { get; set; }

		[MaxLength(4000)]
		public string RequestData { get; set; }

		[MaxLength(4000)]
		public string RefundReason { get; set; }

		public DateTime Timestamp { get; set; }

		[ForeignKey("UserId")]
		public virtual ApplicationUser User { get; set; }

		[ForeignKey("PaytopiaItemId")]
		public virtual PaytopiaItem PaytopiaItem { get; set; }
	}
}
