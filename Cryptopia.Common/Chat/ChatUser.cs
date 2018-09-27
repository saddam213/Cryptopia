using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptopia.Common.Chat
{
	public class ChatUser
	{
		public string Id { get; set; }
		public string UserName { get; set; }
		public string ChatHandle { get; set; }
		public string RoleCss { get; set; }
		public int KarmaTotal { get; set; }
		public DateTime? BanEndTime { get; set; }
		public string Avatar { get; set; }
	}
}
