using Cryptopia.Enums;

namespace Cryptopia.Infrastructure.Common.Email
{
	public class EmailMessageModel
	{
		public string Destination { get; set; }
		public object[] BodyParameters { get; set; }
		public string Subject { get; set; }
		public object SystemIdentifier { get; set; }
		public EmailTemplateType EmailType { get; set; }
	}
}
