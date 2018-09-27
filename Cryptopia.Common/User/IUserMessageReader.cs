using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cryptopia.Common.User
{
	public interface IUserMessageReader
	{
		Task<UserMessageModel> GetMessage(string userId, int messageId);
		Task<List<UserMessageModel>> GetMessages(string userId);
	}
}