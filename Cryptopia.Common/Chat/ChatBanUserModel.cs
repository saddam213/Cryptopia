using Cryptopia.Enums;

namespace Cryptopia.Common.Chat
{
	public class ChatBanUserModel
	{
		public string UserId { get; set; }
		public string BanUserId { get; set; }
		public string Message { get; set; }
		public int Seconds { get; set; }
		public ChatBanType BanType { get; set; }
	}
}