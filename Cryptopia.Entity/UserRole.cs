using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cryptopia.Entity
{
	public class UserRole
	{
		[Key]
		public string UserId { get; set; }

		[Key]
		public string RoleId { get; set; }

		[ForeignKey("RoleId")]
		public virtual IdentityRole Role { get; set; }

		[ForeignKey("UserId")]
		public virtual ApplicationUser User { get; set; }
	}
}
