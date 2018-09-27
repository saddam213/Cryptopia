using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cryptopia.Infrastructure.Common.Email
{
	public interface IEmailService
	{
		Task<bool> SendEmail(EmailMessageModel message);
		Task<bool> SendEmails(EmailMessageModel message, List<IEmailPersonalisation> personalisations);
	}
}