using Cryptopia.Enums;

namespace Cryptopia.Common.Trade
{
	public class CreateTransferModel
	{
		public decimal Amount { get; set; }
		public int CurrencyId { get; set; }
		public string Receiver { get; set; }
		public TransferType TransferType { get; set; }
	}
}