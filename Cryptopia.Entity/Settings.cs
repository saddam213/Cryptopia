using System;
using System.ComponentModel.DataAnnotations;

namespace Cryptopia.Entity
{
	public class Settings
	{
		[Key]
		public int Id { get; set; }
		public decimal PayBanPrice { get; set; }
		public DateTime LastSharePayout { get; set; }
		public DateTime NextSharePayout { get; set; }
		public int ReferralRound { get; set; }
        public DateTime CEFSRound { get; set; }
    }
}