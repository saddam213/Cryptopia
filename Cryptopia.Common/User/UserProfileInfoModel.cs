namespace Cryptopia.Common.User
{
	public class UserProfileInfoModel
	{
		public string UserName { get; set; }
		public double TrustRrating { get; set; }
		public int KarmaPoints { get; set; }
		public string Role { get; set; }
		public int UnreadMessages { get; set; }
		public int UnreadNotifications { get; set; }
	}
}