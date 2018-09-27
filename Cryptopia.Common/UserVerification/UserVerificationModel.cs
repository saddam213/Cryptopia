using Cryptopia.Common.Referral;
using Cryptopia.Common.Validation;
using Cryptopia.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace Cryptopia.Common.UserVerification
{
	public class UserVerificationModel
	{
		[Display(Name = nameof(Resources.User.verificationEmailLabel), ResourceType = typeof(Resources.User))]
		public string Email { get; set; }
		public VerificationLevel VerificationLevel { get; set; }

		[RequiredBase]
		[StringLengthBase(128)]
		[Display(Name = nameof(Resources.User.verificationFirstNameLabel), ResourceType = typeof(Resources.User))]
		public string FirstName { get; set; }

		[RequiredBase]
		[StringLengthBase(128)]
		[Display(Name = nameof(Resources.User.verificationLastNameLabel), ResourceType = typeof(Resources.User))]
		public string LastName { get; set; }

		[RequiredBase]
		[Display(Name = nameof(Resources.User.verificationBirthdayLabel), ResourceType = typeof(Resources.User))]
		public string Birthday { get; set; }

		[RequiredBase]
		[StringLengthBase(128)]
		[Display(Name = nameof(Resources.User.verificationGenderLabel), ResourceType = typeof(Resources.User))]
		public string Gender { get; set; }

		[RequiredBase]
		[StringLengthBase(256)]
		[Display(Name = nameof(Resources.User.verificationAddressLabel), ResourceType = typeof(Resources.User))]
		public string Address { get; set; }

		[RequiredBase]
		[StringLengthBase(128)]
		[Display(Name = nameof(Resources.User.verificationPostcodeLabel), ResourceType = typeof(Resources.User))]
		public string Postcode { get; set; }

		[RequiredBase]
		[StringLengthBase(128)]
		[Display(Name = nameof(Resources.User.verificationCountryLabel), ResourceType = typeof(Resources.User))]
		public string Country { get; set; }

		[RequiredBase]
		[StringLengthBase(128)]
		[Display(Name = nameof(Resources.User.verificationCityLabel), ResourceType = typeof(Resources.User))]
		public string City { get; set; }

		[RequiredBase]
		[StringLengthBase(128)]
		[Display(Name = nameof(Resources.User.verificationStateLabel), ResourceType = typeof(Resources.User))]		
		public string State { get; set; }

		public string Identification1 { get; set; }
		public string Identification2 { get; set; }

		[RequiredToBeTrue(ErrorMessageResourceName = nameof(Resources.User.verificationTermsRequiredError), ErrorMessageResourceType = typeof(Resources.User))]
		public bool TermsAccepted { get; set; }
	}
}
