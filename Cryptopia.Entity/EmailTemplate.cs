using Cryptopia.Enums;
using System.ComponentModel.DataAnnotations;

namespace Cryptopia.Entity
{
	public class EmailTemplate
	{
		[Key]
		public int Id { get; set; }
		public EmailTemplateType Type { get; set; }
		public string Subject { get; set; }
		public string Template { get; set; }
		public bool IsEnabled { get; set; }
		public bool IsHtml { get; set; }
	}
}