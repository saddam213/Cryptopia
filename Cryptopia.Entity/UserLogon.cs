using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cryptopia.Entity
{
	public class UserLogon
	{
		public UserLogon()
		{
			Timestamp = DateTime.UtcNow;
		}

		public UserLogon(string ipAddress)
		{
			IPAddress = ipAddress;
			Timestamp = DateTime.UtcNow;
		}

		[Key]
		public int Id { get; set; }
		public string UserId { get; set; }
		public string IPAddress { get; set; }
		public DateTime Timestamp { get; set; }

		[ForeignKey("UserId")]
		public virtual ApplicationUser User { get; set; }
	}
}