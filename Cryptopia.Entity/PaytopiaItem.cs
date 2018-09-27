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
	public class PaytopiaItem
	{
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		[MaxLength(256)]
		public string Name { get; set; }

		[MaxLength(4000)]
		public string Description { get; set; }

		public decimal Price { get; set; }
		public int CurrencyId { get; set; }

		public PaytopiaItemType Type { get; set; }

		public PaytopiaItemCategory Category { get; set; }

		public PaytopiaItemPeriod Period { get; set; }

		[MaxLength(4000)]
		public string Notes { get; set; }

		public DateTime Timestamp { get; set; }

		public bool IsEnabled { get; set; }


		public virtual ICollection<PaytopiaPayment> Payments { get; set; }
	}
}
