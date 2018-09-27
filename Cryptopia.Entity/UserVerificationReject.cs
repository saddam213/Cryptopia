namespace Cryptopia.Entity
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;

	public class UserVerificationReject
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

		public string RejectReason { get; set; }

		public DateTime Timestamp { get; set; }

		[ForeignKey("UserId")]
		public virtual ApplicationUser User { get; set; }

		public static UserVerificationReject CreateFrom(UserVerification uv)
		{
			return new UserVerificationReject
			{
				UserId = uv.UserId,
				FirstName = uv.FirstName,
				LastName = uv.LastName,
				Address = uv.Address,
				Birthday = uv.Birthday,
				Country = uv.Country,
				City = uv.City,
				Gender = uv.Gender,
				Postcode = uv.Postcode,
				Identification1 = uv.Identification1,
				Identification2 = uv.Identification2,
				State = uv.State,
				Timestamp = DateTime.UtcNow
			};
		}

	}
}