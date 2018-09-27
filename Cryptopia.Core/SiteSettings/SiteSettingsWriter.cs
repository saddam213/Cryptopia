using System.Data.Entity;
using System.Threading.Tasks;
using Cryptopia.Infrastructure.Common.DataContext;
using Cryptopia.Common.SiteSettings;
using Cryptopia.Infrastructure.Common.Results;

namespace Cryptopia.Core.SiteSettings
{
	public class SiteSettingsWriter : ISiteSettingsWriter
	{
		public IDataContextFactory DataContextFactory { get; set; }

		public async Task<IWriterResult> UpdateSiteSettings(SiteSettingsModel model)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var settings = await context.Settings.FirstOrDefaultNoLockAsync().ConfigureAwait(false);
				if (settings == null)
					return new WriterResult(false, "Settings not found");

				settings.PayBanPrice = model.PayBanPrice;

				await context.SaveChangesAsync().ConfigureAwait(false);
				return new WriterResult(true, "Successfully updated site settings.");
			}
		}
	}
}