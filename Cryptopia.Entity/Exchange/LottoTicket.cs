using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cryptopia.Entity
{
	public class LottoTicket 
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }
		public Guid UserId { get; set; }
		public int DrawId { get; set; }
		public int LottoItemId { get; set; }
		public DateTime Timestamp { get; set; }
		public bool IsArchived { get; set; }

		[ForeignKey("UserId")]
		public virtual User User { get; set; }

		[ForeignKey("LottoItemId")]
		public virtual LottoItem LottoItem { get; set; }
	}
}
