namespace Web.Site.Models
{
	public class ViewMessageModel
	{
		public ViewMessageModel()
		{
		}
		public ViewMessageModel(ViewMessageType type, string title, string message, bool showLoginLink = false, bool showSupportLink = false, string returnUrl = "/", bool showContinueLink = false, string continueLink="/", string continueLinkText = "Continue")
		{
			Type = type;
			Title = title;
			Message = message;
			ShowLoginLink = showLoginLink;
			ShowSupportLink = showSupportLink;
			ReturnUrl = returnUrl;

			ContinueLink = continueLink;
			ContinueLinkText = continueLinkText;
			ShowContinueLink = showContinueLink;
		}

		public bool ShowLoginLink { get; set; }
		public bool ShowSupportLink { get; set; }
		public ViewMessageType Type { get; set; }
		public string Title { get; set; }
		public string Message { get; set; }
		public string ReturnUrl { get; set; }

		public bool ShowContinueLink { get; set; }
		public string ContinueLink { get; set; }
		public string ContinueLinkText { get; set; }

	}

	public enum ViewMessageType
	{
		Info,
		Success,
		Warning,
		Danger
	}
}