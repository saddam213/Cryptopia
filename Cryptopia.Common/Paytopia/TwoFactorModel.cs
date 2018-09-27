using Cryptopia.Common.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptopia.Common.Paytopia
{
	public class TwoFactorModel
	{
		public decimal Balance { get; set; }
		public string Currency { get; set; }
		public string Description { get; set; }
		public string Name { get; set; }
		public decimal Price { get; set; }

		[RequiredBase]
		[StringLengthBase(128)]
		[Display(Name = nameof(Resources.Paytopia.twoFactorRecipientLabel), ResourceType = typeof(Resources.Paytopia))]
		public string Recipient { get; set; }

		[RequiredBase]
		[StringLengthBase(128)]
		[Display(Name = nameof(Resources.Paytopia.twoFactorStreetAddressLabel), ResourceType = typeof(Resources.Paytopia))]
		public string Street { get; set; }

		[RequiredBase]
		[StringLengthBase(128)]
		[Display(Name = nameof(Resources.Paytopia.twoFactorCityStateLabel), ResourceType = typeof(Resources.Paytopia))]
		public string City { get; set; }

		[RequiredBase]
		[StringLengthBase(128)]
		[Display(Name = nameof(Resources.Paytopia.twoFactorPostCodeLabel), ResourceType = typeof(Resources.Paytopia))]
		public string PostCode { get; set; }

		[RequiredBase]
		[Display(Name = nameof(Resources.Paytopia.twoFactorCountryLabel), ResourceType = typeof(Resources.Paytopia))]
		public string Country { get; set; }
	}
}
