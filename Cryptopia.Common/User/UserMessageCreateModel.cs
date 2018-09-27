using Cryptopia.Common.Validation;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Cryptopia.Common.User
{
	public class UserMessageCreateModel
	{
		[RequiredBase]
		[StringLengthBase(200)]
		public string Subject { get; set; }

		[RequiredBase]
		[StringLengthBase(1000)]
		public string Recipiants { get; set; }

		[RequiredBase]
		[AllowHtml]
		[UIHint("tinymce_jquery_full")]
		[StringLengthBase(10000)]
		public string Message { get; set; }
	}
}