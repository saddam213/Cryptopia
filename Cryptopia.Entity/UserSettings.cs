using Cryptopia.Enums;
using System.ComponentModel.DataAnnotations;

namespace Cryptopia.Entity
{
	public class UserSettings
	{
		public UserSettings()
		{
			Theme = SiteTheme.Light;
			DefaultMineShaft = 0;
			DefaultTradepair = 0;
		}

		[Key]
		public string Id { get; set; }

		public int DefaultTradepair { get; set; }
		public int DefaultMineShaft { get; set; }

		public SiteTheme Theme { get; set; }
		public bool HideZeroBalance { get; set; }
		public bool ShowFavoriteBalance { get; set; }
	}
}