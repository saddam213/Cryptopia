using System.Collections.Generic;
using Cryptopia.Common.Chat;
using Cryptopia.Common.Emoticons;

namespace Web.Site.Models.Chat
{
	public class ChatEmoticonViewModel
	{
		public ChatEmoticonViewModel()
		{
			Emoticons = new List<EmoticonModel>();
		}

		public List<EmoticonModel> Emoticons { get; set; }

		public List<string> Categories { get; set; }
	}

	
}