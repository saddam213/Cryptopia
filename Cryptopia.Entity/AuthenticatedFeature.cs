using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Cryptopia.Enums;

namespace Cryptopia.Entity
{
	public class AuthenticatedFeature
	{
		[Key]
		public int Id { get; set; }

		public AuthenticatedFeatureType AuthenticatedFeatureType { get; set; }

		public int TwoFactorCodeId { get; set; }

		[ForeignKey("TwoFactorCodeId")]
		public virtual TwoFactorCode TwoFactorCode { get; set; }
	}
}