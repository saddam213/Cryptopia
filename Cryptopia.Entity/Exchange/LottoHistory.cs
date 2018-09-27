using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cryptopia.Entity
{
	public class LottoHistory
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }
		public Guid UserId { get; set; }
		public int LottoItemId { get; set; }
		public int LottoDrawId { get; set; }
		public int LottoTicketId { get; set; }
		public decimal TotalAmount { get; set; }
		public decimal Amount { get; set; }
		public int Position { get; set; }
		public decimal Percent { get; set; }
		public DateTime Timestamp { get; set; }

		[ForeignKey("UserId")]
		public virtual User User { get; set; }

		[ForeignKey("LottoItemId")]
		public virtual LottoItem LottoItem { get; set; }

		[ForeignKey("LottoTicketId")]
		public virtual LottoTicket Ticket { get; set; }
	}
}
