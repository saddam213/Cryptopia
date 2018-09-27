using Cryptopia.Common.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptopia.Common.Paytopia
{
	public class SharesModel
	{
		public decimal Balance { get; set; }
		public string Currency { get; set; }
		public string Description { get; set; }
		public string Name { get; set; }
		public decimal Price { get; set; }
	
		[RangeBase(1, 20000)]
		[Display(Name = nameof(Resources.Paytopia.sharesCountLabel), ResourceType = typeof(Resources.Paytopia))]
		public int Count { get; set; }

		[RequiredBase]
		[StringLengthBase(128)]
		[Display(Name = nameof(Resources.Paytopia.sharesFirstNameLabel), ResourceType = typeof(Resources.Paytopia))]
		public string FirstName { get; set; }

		[StringLengthBase(128)]
		[Display(Name = nameof(Resources.Paytopia.sharesMiddleNameLabel), ResourceType = typeof(Resources.Paytopia))]
		public string MiddleName { get; set; }

		[RequiredBase]
		[StringLengthBase(128)]
		[Display(Name = nameof(Resources.Paytopia.sharesLastNameLabel), ResourceType = typeof(Resources.Paytopia))]
		public string LastName { get; set; }

		[RequiredBase]
		[StringLengthBase(128)]
		[Display(Name = nameof(Resources.Paytopia.sharesStreetAddressLabel), ResourceType = typeof(Resources.Paytopia))]
		public string Street { get; set; }

		[RequiredBase]
		[StringLengthBase(128)]
		[Display(Name = nameof(Resources.Paytopia.sharesCityStateLabel), ResourceType = typeof(Resources.Paytopia))]
		public string City { get; set; }

		[RequiredBase]
		[StringLengthBase(128)]
		[Display(Name = nameof(Resources.Paytopia.sharesPostCodeLabel), ResourceType = typeof(Resources.Paytopia))]
		public string PostCode { get; set; }

		[RequiredBase]
		[Display(Name = nameof(Resources.Paytopia.sharesCountryLabel), ResourceType = typeof(Resources.Paytopia))]		
		public string Country { get; set; }

		[RequiredBase]
		[EmailAddress]
		[StringLengthBase(128)]
		[Display(Name = nameof(Resources.Paytopia.sharesEmailsAddressLabel), ResourceType = typeof(Resources.Paytopia))]
		public string Email { get; set; }

		[RequiredBase]
		[StringLengthBase(128)]
		[Display(Name = nameof(Resources.Paytopia.sharesPhoneLabel), ResourceType = typeof(Resources.Paytopia))]		
		public string Phone { get; set; }
	}
}
