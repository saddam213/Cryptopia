using Cryptopia.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptopia.Common.Paytopia
{

	public class AdminUpdatePaytopiaPaymentModel
	{
		public int PaymentId { get; set; }
		public PaytopiaPaymentStatus Status { get; set; }
		public string Reason { get; set; }
	}
}
