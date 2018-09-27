using System.Threading.Tasks;
using Cryptopia.Infrastructure.Common.Results;

namespace Cryptopia.Common.SiteSettings
{
	public interface ISiteSettingsWriter
	{
		Task<IWriterResult> UpdateSiteSettings(SiteSettingsModel model);
	}
}
