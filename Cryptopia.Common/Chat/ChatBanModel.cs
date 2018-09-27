using System.Collections.Generic;

namespace Cryptopia.Common.Chat
{
	public class ChatBanModel
	{
		public string UserId { get; set; }
		public List<ChatUserModel> ChatUsers { get; set; }
	}
}