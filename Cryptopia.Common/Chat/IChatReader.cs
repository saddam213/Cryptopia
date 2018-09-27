using System.Collections.Generic;
using System.Threading.Tasks;
using Cryptopia.Infrastructure.Common.DataTables;

namespace Cryptopia.Common.Chat
{
	public interface IChatReader
	{
		Task<List<ChatUserModel>> GetChatUsers();
		Task<DataTablesResponse> GetBannedUsers(DataTablesModel model);
		Task<DataTablesResponse> GetChatMessages(DataTablesModel model);
	}
}