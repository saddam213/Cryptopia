using System.Threading.Tasks;

using Cryptopia.Infrastructure.Incapsula.Common.Classes;
using Cryptopia.Infrastructure.Incapsula.Common.Enums;

namespace Cryptopia.Admin.Common.Incapsula
{
    public interface IIncapsulaWriter
    {
        Task<IPBlacklist> BlacklistIpAddress(string adminUserId, string ipAddress);
        Task<ResponseCode> PurgeSiteCache(string adminUserId);
    }
}
