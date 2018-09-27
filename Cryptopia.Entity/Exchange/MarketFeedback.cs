using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cryptopia.Entity
{
	public class MarketFeedback
	{
		[Key]
		public int Id { get; set; }

		public int MarketItemId { get; set; }
		public Guid SenderUserId { get; set; }
		public Guid ReceiverUserId { get; set; }
		public int Rating { get; set; }

		[MaxLength(256)]
		public string Comment { get; set; }

		public DateTime Timestamp { get; set; }


		[ForeignKey("MarketItemId")]
		public virtual Entity.MarketItem MarketItem { get; set; }

		[ForeignKey("SenderUserId")]
		public virtual Entity.User SenderUser { get; set; }

		[ForeignKey("ReceiverUserId")]
		public virtual Entity.User ReceiverUser { get; set; }
	}
}