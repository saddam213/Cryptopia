
using System.ComponentModel;

namespace Cryptopia.Admin.Common.Reprocessing
{
    public class ReprocessingOptionsModel
    {
        [DisplayName("Search Block Length")]
        public int WalletSearchBlockLength { get; set; } = 10000;
        
    }
}