using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cryptopia.Entity
{
	public class UserVerification
	{
		[Key]
		public int Id { get; set; }

		public string UserId { get; set; }

		[MaxLength(128)]
		public string FirstName { get; set; }

		[MaxLength(128)]
		public string LastName { get; set; }

		[MaxLength(256)]
		public string Address { get; set; }

		[MaxLength(128)]
		public string Postcode { get; set; }

		[MaxLength(128)]
		public string Country { get; set; }

		[MaxLength(128)]
		public string City { get; set; }

		[MaxLength(128)]
		public string State { get; set; }

		[MaxLength(128)]
		public string Gender { get; set; }

		[MaxLength(128)]
		public string Birthday { get; set; }

		public string Identification1 { get; set; }
		public string Identification2 { get; set; }

		public string ApprovedBy { get; set; }
		public DateTime? Approved { get; set; }
		public DateTime Timestamp { get; set; }

		[ForeignKey("UserId")]
		public virtual ApplicationUser User { get; set; }

		[ForeignKey("ApprovedBy")]
		public virtual ApplicationUser ApprovedByUser { get; set; }
	}
}