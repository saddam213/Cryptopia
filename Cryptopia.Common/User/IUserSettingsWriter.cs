using Cryptopia.Enums;
using System.Threading.Tasks;
using Cryptopia.Infrastructure.Common.Results;

namespace Cryptopia.Common.User
{
	public interface IUserSettingsWriter
	{
		Task<WriterResult> UpdateSettings(string userId, UserSettingsModel model);
		Task<WriterResult> UpdateTheme(string userId, SiteTheme theme);
		Task<WriterResult<bool>> UpdateIgnoreList(string userId, UserIgnoreListType type, string handle);
		Task<WriterResult> UpdateChartSettings(string userId, string settings);
		Task<IWriterResult> UpdateBalanceHideZero(string userId, bool hideZero);
		Task<IWriterResult> UpdateBalanceFavoriteOnly(string userId, bool favoritesOnly);
	}
}