using System.Threading.Tasks;

using Cryptopia.Admin.Common.Incapsula;
using Cryptopia.Admin.Core.AdmintopiaService;
using Cryptopia.Infrastructure.Incapsula.Common.Classes;
using Cryptopia.Infrastructure.Incapsula.Common.Enums;
using Cryptopia.Infrastructure.Common.DataContext;

namespace Cryptopia.Admin.Core.Incapsula
{
    public class IncapsulaWriter : IIncapsulaWriter
    {
        public IDataContextFactory DataContextFactory { get; set; }

        public async Task<IPBlacklist> BlacklistIpAddress(string adminUserId, string ipAddress)
        {
            using (var service = new AdmintopiaServiceClient())
            {
                var newIpBlacklist = await service.BlacklistIpAddressAsync(ipAddress);

                using (var context = DataContextFactory.CreateContext())
                {
                    context.LogActivity(adminUserId, $"Blacklisted IP Address: {ipAddress}");
                    await context.SaveChangesAsync().ConfigureAwait(false);
                }

                return newIpBlacklist;
            }
        }

        public async Task<ResponseCode> PurgeSiteCache(string adminUserId)
        {
            using (var service = new AdmintopiaServiceClient())
            {
                using (var context = DataContextFactory.CreateContext())
                {
                    context.LogActivity(adminUserId, $"Purged website cache");
                    await context.SaveChangesAsync().ConfigureAwait(false);
                }

                return await service.PurgeSiteCacheAsync();
            }
        }
    }
}
