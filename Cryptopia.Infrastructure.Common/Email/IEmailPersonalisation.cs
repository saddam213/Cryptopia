using System.Collections.Generic;

namespace Cryptopia.Infrastructure.Common.Email
{
	public interface IEmailPersonalisation
	{
		List<string> Tos { get; set; }
		List<string> Ccs { get; set; }
		List<string> Bccs { get; set; }
		string Subject { get; set; }
		Dictionary<string, string> Headers { get; set; }
		Dictionary<string, string> Substitutions { get; set; }
	}
}