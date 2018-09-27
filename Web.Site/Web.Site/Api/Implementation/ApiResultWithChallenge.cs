using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace Web.Site.Api.Authentication
{
	public class ApiResultWithChallenge : IHttpActionResult
	{
		private readonly string _authenticationScheme = "amx";
		private readonly IHttpActionResult _next;

		public ApiResultWithChallenge(IHttpActionResult next)
		{
			this._next = next;
		}

		public async Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
		{
			var response = await _next.ExecuteAsync(cancellationToken).ConfigureAwait(false);
			if (response.StatusCode == HttpStatusCode.Unauthorized)
			{
				response.Headers.WwwAuthenticate.Add(new AuthenticationHeaderValue(_authenticationScheme));
			}
			return response;
		}
	}
}