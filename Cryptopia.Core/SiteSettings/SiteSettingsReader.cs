using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Cryptopia.Infrastructure.Common.DataContext;
using Cryptopia.Common.SiteSettings;

namespace Cryptopia.Core.SiteSettings
{
	public class SiteSettingsReader : ISiteSettingsReader
	{
		public IDataContextFactory DataContextFactory { get; set; }

		public async Task<SiteSettingsModel> GetSiteSettings()
		{
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				return await context.Settings
				.AsNoTracking()
				.Select(x => new SiteSettingsModel
				{
					PayBanPrice = x.PayBanPrice
				}).FirstOrDefaultNoLockAsync().ConfigureAwait(false);
			}
		}
	}
}
