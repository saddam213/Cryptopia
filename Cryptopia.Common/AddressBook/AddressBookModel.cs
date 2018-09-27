using Cryptopia.Common.Currency;
using Cryptopia.Common.Validation;
using Cryptopia.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cryptopia.Common.Address
{
	public class AddressBookModel
	{
		public int Id { get; set; }
		public int CurrencyId { get; set; }
		public string Symbol { get; set; }

		[RequiredBase]
		public string Label { get; set; }
		public string Address { get; set; }
		public string PaymentId { get; set; }
		public string BankAccount { get; set; }
		public string BankReference { get; set; }


		public List<CurrencyModel> Currencies { get; set; }
		public CurrencyType CurrencyType { get; set; }
		public AddressType AddressType { get; set; }
	}
}
