using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Cryptopia.Common.Chat
{
	public class ChatMessageData
	{
		public int Id { get; set; }
		public bool IsEnabled { get; set; }
		public string Message { get; set; }
		public DateTime Timestamp { get; set; }
		public string UserId { get;  set; }
		public int KarmaTotal { get; set; }
		public string Avatar { get; set; }
		public string Flair { get; set; }
		public string UserName { get; set; }
		public string ChatHandle { get; set; }
		public bool IsTipBot { get; set; }
		public bool IsBot { get; set; }
	}
}
