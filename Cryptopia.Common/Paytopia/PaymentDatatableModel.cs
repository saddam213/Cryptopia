using Cryptopia.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptopia.Common.Paytopia
{
	public class PaymentDatatableModel
	{
		public int Id { get; set; }
		public PaytopiaItemType Type { get; set; }
		public decimal Amount { get; set; }
		public string Symbol { get; set; }
		public PaytopiaPaymentStatus Status { get; set; }
		public string UserName { get; set; }
		public bool IsAnonymous { get; set; }
		public DateTime Begins { get; set; }
		public DateTime Ends { get; set; }
		public int TransferId { get; set; }
		public int RefundId { get; set; }
		public DateTime Timestamp { get; set; }
		public int CurrencyId { get; set; }
		
	}

	public class PaytopiaPaymentModel
	{
		public int Id { get; set; }
		public PaytopiaItemType Type { get; set; }
		public int CurrencyId { get; set; }
		public decimal Amount { get; set; }
		public PaytopiaPaymentStatus Status { get; set; }
		public string UserName { get; set; }
		public bool IsAnonymous { get; set; }

		public DateTime Begins { get; set; }
		public DateTime Ends { get; set; }
		public DateTime Timestamp { get; set; }
		public int TransferId { get; set; }
		public string Symbol { get; set; }

		public string ReferenceCode { get; set; }
		public int ReferenceId { get; set; }
		public string RefundReason { get; set; }
		public string RequestData { get; set; }
		public string ReferenceSymbol { get; set; }
		public AlgoType ReferenceAlgo { get; set; }
		public string ReferenceName { get; set; }
		public int RefundId { get; set; }
	}
}
