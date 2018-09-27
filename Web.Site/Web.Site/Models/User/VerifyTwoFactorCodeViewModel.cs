using System.ComponentModel.DataAnnotations;
using Cryptopia.Enums;

namespace Web.Site.Models
{
	public class VerifyTwoFactorCodeViewModel
    {
        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }

        public string TwoFactorData { get; set; }
        public string ExtraTwoFactorData { get; set; }
        public TwoFactorType TwoFactorType { get; set; }
        public TwoFactorComponent Component { get; set; }
		public bool DisableEmailConfirmation { get; set; }
    }
}