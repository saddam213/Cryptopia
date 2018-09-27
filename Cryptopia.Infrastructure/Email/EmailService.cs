using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Cryptopia.Base.Logging;
using Cryptopia.Infrastructure.Common.DataContext;
using Cryptopia.Infrastructure.Common.Email;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Cryptopia.Infrastructure.Email
{
	public class EmailService : IEmailService
	{
		private static readonly string ApiKey = ConfigurationManager.AppSettings["Email_ApiKey"];
		private static readonly string EmailFrom = ConfigurationManager.AppSettings["Email_NoReply"];
		private readonly ISendGridClient _sendGridClient;

		public EmailService()
		{
			_sendGridClient = new SendGridClient(ApiKey);
		}

		public IDataContextFactory DataContextFactory { get; set; }

		public async Task<bool> SendEmail(EmailMessageModel message)
		{
			try
			{
				using (var context = DataContextFactory.CreateContext())
				{
					var template = await context.EmailTemplates.FirstOrDefaultNoLockAsync(x => x.Type == message.EmailType);
					if (template == null)
						return false;

					if (string.IsNullOrEmpty(message.Subject))
						message.Subject = template.Subject;

					var body = string.Format(template.Template, message.BodyParameters).Replace("{{", "{").Replace("}}", "}");

					var email = new SendGridMessage
					{
						From = new EmailAddress(EmailFrom),
						Subject = message.Subject,
						HtmlContent = body,
						Headers = new Dictionary<string, string>(),
						Categories = new List<string>()
					};

					email.Headers.Add("X-SMTPAPI", $"{{\"category\": [\"{message.EmailType} - {message.SystemIdentifier}\"]}}");
					email.Categories.Add($"{message.EmailType} -- {message.SystemIdentifier}");
					email.AddTo(message.Destination);
					var response = await _sendGridClient.SendEmailAsync(email).ConfigureAwait(false);
					return response.StatusCode == HttpStatusCode.Accepted;
				}
			}
			catch (Exception)
			{
				return false;
			}
		}

		public async Task<bool> SendEmails(EmailMessageModel message, List<IEmailPersonalisation> personalisations)
		{
			if (!personalisations.Any())
				return false;

			try
			{
				using (var context = DataContextFactory.CreateContext())
				{
					var template = await context.EmailTemplates.FirstOrDefaultNoLockAsync(x => x.Type == message.EmailType);
					if (template == null)
						return false;

					if (string.IsNullOrEmpty(message.Subject))
						message.Subject = template.Subject;

					var body = string.Format(template.Template, message.BodyParameters).Replace("{{", "{").Replace("}}", "}");

					var email = new SendGridMessage
					{
						From = new EmailAddress(EmailFrom),
						Subject = message.Subject,
						HtmlContent = body,
						Headers = new Dictionary<string, string> { { "category", $"{message.EmailType} - {message.SystemIdentifier}" } },
						Personalizations = personalisations.Select(p => new Personalization
						{
							Bccs = p.Bccs?.Select(e => new EmailAddress(e)).ToList(),
							Ccs = p.Ccs?.Select(e => new EmailAddress(e)).ToList(),
							Substitutions = p.Substitutions,
							Tos = p.Tos?.Select(e => new EmailAddress(e)).ToList(),
							Subject = string.IsNullOrWhiteSpace(p.Subject) ? message.Subject : p.Subject
						}).ToList()
					};
					
					var response = await _sendGridClient.SendEmailAsync(email).ConfigureAwait(false);
					return response.StatusCode == HttpStatusCode.Accepted;
				}
			}
			catch (Exception)
			{
				return false;
			}
		}
	}
}