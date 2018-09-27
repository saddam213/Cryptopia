using System;

namespace Cryptopia.Admin.Common.AdminUser
{
	public class AdminUserProfileUpdateModel
	{
		public string Id { get; set; }
		public string UserName { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public DateTime Birthday { get; set; }
		public string Gender { get; set; }
		public string Address { get; set; }
		public string City { get; set; }
		public string State { get; set; }
		public string Postcode { get; set; }
		public string Country { get; set; }
		public string ContactEmail { get; set; }
		public string Occupation { get; set; }

		public string Education { get; set; }
		public string Facebook { get; set; }
		public string Hobbies { get; set; }
		public string AboutMe { get; set; }
		public string LinkedIn { get; set; }
		public string Twitter { get; set; }
		public string Website { get; set; }
		public bool IsPublic { get; set; }

	}
}