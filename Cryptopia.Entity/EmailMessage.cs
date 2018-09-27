using Cryptopia.Enums;
using System.ComponentModel.DataAnnotations;
using System;

namespace Cryptopia.Entity
{
	public class EmailMessage
	{
		[Key]
		public int Id { get; set; }
		public EmailTemplateType Type { get; set; }
		public string Subject { get; set; }
		public string Body { get; set; }
		public string Destination { get; set; }
		public DateTime Timestamp { get; set; }
		public bool Sent { get; set; }
	}
}