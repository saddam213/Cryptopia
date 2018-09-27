using System.ComponentModel.DataAnnotations;

namespace Cryptopia.Common.User
{
	public class UpdateApiModel
	{
		public bool IsApiEnabled { get; set; }

		[Required]
		public string ApiKey { get; set; }

		[Required]
		public string ApiSecret { get; set; }

		public bool IsApiWithdrawEnabled { get; set; }
		public bool IsApiUnsafeWithdrawEnabled { get; set; }
		public string OldApiKey { get; set; }
	}
}
