using Cryptopia.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cryptopia.Entity
{
	public class TwoFactorCode
	{
		[Key]
		public int Id { get; set; }

		public string UserId { get; set; }

		public string SerialNumber { get; set; }

		public string OriginalData { get; set; }

		public string CurrentData { get; set; }
	}
}