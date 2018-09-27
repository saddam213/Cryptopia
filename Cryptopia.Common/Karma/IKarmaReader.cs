using System.Threading.Tasks;
using Cryptopia.Infrastructure.Common.DataTables;

namespace Cryptopia.Common.Karma
{
	public interface IKarmaReader
	{
		Task<UserKarmaModel> GetUserKarma(string userId);
		Task<DataTablesResponse> GetKarmaHistory(string userId, DataTablesModel model);
		Task<DataTablesResponse> GetKarmaReceived(string userId, DataTablesModel model);
	}
}