using Cryptopia.Common.Referral;
using System;
using System.ComponentModel.DataAnnotations;
using Cryptopia.Enums;
using Cryptopia.Common.Validation;

namespace Cryptopia.Common.User
{
	public class UserProfileModel
	{
		public UserProfileModel()
		{
			ReferralDetails = new ReferralInfoModel();
		}

		[RequiredBase]
		[StringLengthBase(15)]
		[RegularExpression(@"^\w+$", ErrorMessageResourceName = nameof(Resources.User.profileChatHandleError), ErrorMessageResourceType = typeof(Resources.User))]
		[Display(Name = nameof(Resources.User.profileChatHandleLabel), ResourceType = typeof(Resources.User))]
		public string ChatHandle { get; set; }

		[RequiredBase]
		[StringLengthBase(15)]
		[RegularExpression(@"^\w+$", ErrorMessageResourceName = nameof(Resources.User.profileMiningHandleError), ErrorMessageResourceType = typeof(Resources.User))]
		[Display(Name = nameof(Resources.User.profileMiningHandleLabel), ResourceType = typeof(Resources.User))]
		public string MiningHandle { get; set; }

		[StringLengthBase(256)]
		[Display(Name = nameof(Resources.User.profileFirstNameLabel), ResourceType = typeof(Resources.User))]
		public string FirstName { get; set; }

		[StringLengthBase(256)]
		[Display(Name = nameof(Resources.User.profileLastNameLabel), ResourceType = typeof(Resources.User))]
		public string LastName { get; set; }

		[StringLength(50)]
		[Display(Name = nameof(Resources.User.profileCountryLabel), ResourceType = typeof(Resources.User))]
		public string Country { get; set; }

		[EmailAddress(ErrorMessageResourceName = nameof(Resources.Validation.attributeEmailAddressError), ErrorMessageResourceType = typeof(Resources.Validation))]
		[Display(Name = nameof(Resources.User.profileContactEmailLabel), ResourceType = typeof(Resources.User))]		
		public string ContactEmail { get; set; }

		[StringLengthBase(5000)]
		[Display(Name = nameof(Resources.User.profileAboutMeLabel), ResourceType = typeof(Resources.User))]
		public string AboutMe { get; set; }

		[StringLengthBase(50)]		
		[Display(Name = nameof(Resources.User.profileGenderLabel), ResourceType = typeof(Resources.User))]
		public string Gender { get; set; }

		[DataType(DataType.Date)]
		[Display(Name = nameof(Resources.User.profileBirthdayLabel), ResourceType = typeof(Resources.User))]		
		public DateTime Birthday { get; set; }

		[StringLengthBase(256)]
		[Display(Name = nameof(Resources.User.profileOccupationLabel), ResourceType = typeof(Resources.User))]
		public string Occupation { get; set; }

		[MaxLength(1024)]
		public string Hobbies { get; set; }

		[StringLengthBase(1024)]
		[Display(Name = nameof(Resources.User.profileEducationLabel), ResourceType = typeof(Resources.User))]
		public string Education { get; set; }

		[Url(ErrorMessageResourceName = nameof(Resources.Validation.attributeUrlError), ErrorMessageResourceType = typeof(Resources.Validation))]
		[StringLengthBase(256)]
		[Display(Name = nameof(Resources.User.profileWebsiteLabel), ResourceType = typeof(Resources.User))]
		public string Website { get; set; }

		[Url(ErrorMessageResourceName = nameof(Resources.Validation.attributeUrlError), ErrorMessageResourceType = typeof(Resources.Validation))]
		[MaxLength(256)]
		public string Facebook { get; set; }

		[Url(ErrorMessageResourceName = nameof(Resources.Validation.attributeUrlError), ErrorMessageResourceType = typeof(Resources.Validation))]
		[StringLengthBase(256)]
		public string Twitter { get; set; }

		[Url(ErrorMessageResourceName = nameof(Resources.Validation.attributeUrlError), ErrorMessageResourceType = typeof(Resources.Validation))]
		[MaxLength(256)]
		public string LinkedIn { get; set; }

		// Un editable
		[EmailAddress(ErrorMessageResourceName = nameof(Resources.Validation.attributeEmailAddressError), ErrorMessageResourceType = typeof(Resources.Validation))]
		[Display(Name = nameof(Resources.User.profileAccountEmailLabel), ResourceType = typeof(Resources.User))]
		public string AccountEmail { get; set; }

		[Display(Name = nameof(Resources.User.profileIsPublicLabel), ResourceType = typeof(Resources.User))]
		public bool IsPublic { get; set; }

		public double TrustRating { get; set; }
		public ReferralInfoModel ReferralDetails { get; set; }
		public VerificationLevel VerificationLevel { get; set; }

		[StringLengthBase(256)]
		[Display(Name = nameof(Resources.User.profileAddressLabel), ResourceType = typeof(Resources.User))]
		public string Address { get; set; }
		
		[StringLengthBase(256)]
		[Display(Name = nameof(Resources.User.profilePostcodeLabel), ResourceType = typeof(Resources.User))]
		public string Postcode { get; set; }
		
		[StringLengthBase(256)]
		[Display(Name = nameof(Resources.User.profileCityLabel), ResourceType = typeof(Resources.User))]
		public string City { get; set; }
		
		[StringLengthBase(256)]
		[Display(Name = nameof(Resources.User.profileStateLabel), ResourceType = typeof(Resources.User))]
		public string State { get; set; }
	}
}
