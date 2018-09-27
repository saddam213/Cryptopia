namespace Web.Site.Api
{
	public class ApiErrorResponse
	{
		public ApiErrorResponse()
		{
			Success = false;
		}

		public ApiErrorResponse(string error)
		{
			Success = false;
			Error = error;
		}

		public bool Success { get; set; }
		public string Error { get; set; }
	}
}