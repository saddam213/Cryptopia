
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Cryptopia.Admin.Common.Incapsula
{
    public class BlockIpAddressModel
    {
        [Required]
        [DisplayName("IP Address")]
        [RegularExpression(@"^(?:[0-9]{1,3}\.){3}[0-9]{1,3}$")]
        public string Address { get; set; }

        [Required]
        [DisplayName("Two Factor Code")]
        public string AuthenticationCode { get; set; }
    }
}
