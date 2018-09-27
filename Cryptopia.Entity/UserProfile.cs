using Cryptopia.Entity.Auditing;
using System;
using System.ComponentModel.DataAnnotations;

namespace Cryptopia.Entity
{
	public class UserProfile
	{
		public UserProfile()
		{
			// Bitcoins birthday :)
			Birthday = new DateTime(2009, 1, 3);
		}

		[Key]
		public string Id { get; set; }

		public bool IsPublic { get; set; }

		[MaxLength(50)]
		public string FirstName { get; set; }

		[MaxLength(50)]
		public string LastName { get; set; }

		[MaxLength(256)]
		public string Address { get; set; }

		[MaxLength(50)]
		public string Postcode { get; set; }

		[MaxLength(256)]
		public string Country { get; set; }

		[MaxLength(256)]
		public string City { get; set; }

		[MaxLength(256)]
		public string State { get; set; }

		[MaxLength(256)]
		public string ContactEmail { get; set; }

		[MaxLength(4086)]
		public string AboutMe { get; set; }

		[MaxLength(50)]
		public string Gender { get; set; }

		public DateTime Birthday { get; set; }

		[MaxLength(1024)]
		public string Occupation { get; set; }

		[MaxLength(1024)]
		public string Hobbies { get; set; }

		[MaxLength(1024)]
		public string Education { get; set; }

		[MaxLength(256)]
		public string Website { get; set; }

		[MaxLength(256)]
		public string Facebook { get; set; }

		[MaxLength(256)]
		public string Twitter { get; set; }

		[MaxLength(256)]
		public string LinkedIn { get; set; }
	}
}