using System.Threading.Tasks;

namespace Cryptopia.Common.SiteSettings
{
	public interface ISiteSettingsReader
	{
		Task<SiteSettingsModel> GetSiteSettings();
	}
}
