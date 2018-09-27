using Cryptopia.Enums;

namespace Cryptopia.Common.Karma
{
	public class CreateKarmaModel
	{
		public string Discriminator { get; set; }
		public UserKarmaType Type { get; set; }
		public string UserId { get; set; }
	}
}