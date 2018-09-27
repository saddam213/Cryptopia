using System.Threading.Tasks;
using Cryptopia.Infrastructure.Common.Results;

namespace Cryptopia.Common.Pool
{
	public interface IPoolWriter
	{
		Task<IWriterResult> AdminUpdatePool(AdminUpdatePoolModel model);
		Task<IWriterResult> AdminUpdatePoolSettings(AdminUpdatePoolSettingsModel model);
		Task<IWriterResult> AdminUpdatePoolConnection(AdminUpdatePoolConnectionModel model);
	}
}
