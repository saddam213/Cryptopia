using System.Collections.Generic;
using Cryptopia.Infrastructure.Common.Email;

namespace Cryptopia.Infrastructure.Email
{
	public class EmailPersonalisation : IEmailPersonalisation
	{
		public List<string> Tos { get; set; }
		public List<string> Ccs { get; set; }
		public List<string> Bccs { get; set; }
		public string Subject { get; set; }
		public Dictionary<string, string> Headers { get; set; }
		public Dictionary<string, string> Substitutions { get; set; }
	}
}