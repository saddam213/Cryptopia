namespace Cryptopia.Admin.Common.UserVerification
{
	using System;

	public class UserDetailsModel
	{
		//UserVerification
		public int VerificationId { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Address { get; set; }
		public string Postcode { get; set; }
		public string Country { get; set; }
		public string City { get; set; }
		public string State { get; set; }
		public string Gender { get; set; }
		public string Birthday { get; set; }

		public string Email { get; set; }

		public string Identification1 { get; set; }
		public string Identification2 { get; set; }

		public bool AdminCanVerify { get; set; }
		public string RejectReason { get; set; }
	}
}