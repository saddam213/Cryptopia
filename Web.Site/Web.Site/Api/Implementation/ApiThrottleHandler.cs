using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using WebApiThrottle;

namespace Web.Site.Api.Implementation
{
	public class ApiThrottleHandler : ThrottlingHandler
	{

		protected override RequestIdentity SetIdentity(HttpRequestMessage request)
		{
			var key = "Public";
			if (request.Headers.Contains("Authorization"))
			{
				var apiKey = request.Headers.GetValues("Authorization").First();
				if (apiKey.Contains(':'))
				{
					key = apiKey.Split(':')[0].Replace("amx", "").Trim();
				}
			}

			return new RequestIdentity()
			{
				ClientKey = key,
				ClientIp = base.GetClientIp(request).ToString(),
				Endpoint = request.RequestUri.AbsolutePath.ToLowerInvariant()
			};
		}
		/// <summary>
		/// Override QuotaExceededResponse so we can return a compatable API error resopnse
		/// </summary>
		/// <param name="request">The request.</param>
		/// <param name="message">The message.</param>
		/// <param name="responseCode">The response code.</param>
		/// <param name="retryAfter">The retry after.</param>
		protected override Task<HttpResponseMessage> QuotaExceededResponse(HttpRequestMessage request, object message, HttpStatusCode responseCode, string retryAfter)
		{
			var response = request.CreateResponse<ThrottleResponse>(responseCode, new ThrottleResponse { Success = false, Error = (message ?? string.Empty).ToString() });
			response.Headers.Add("Retry-After", new string[] { retryAfter });
			return Task.FromResult(response);
		}

	
	}

	[DataContract]
	public class ThrottleResponse
	{
		[DataMember(Order = 0)]
		public bool Success { get; set; }

		[DataMember(Order = 1)]
		public string Error { get; set; }
	}

	public class ApiThrottleFilter : ThrottlingFilter
	{
		protected override RequestIdentity SetIdentity(HttpRequestMessage request)
		{
			var key = "Public";
			if (request.Headers.Contains("Authorization"))
			{
				var apiKey = request.Headers.GetValues("Authorization").First();
				if (apiKey.Contains(':'))
				{
					key = apiKey.Split(':')[0].Replace("amx", "").Trim();
				}
			}

			return new RequestIdentity()
			{
				ClientKey = key,
				ClientIp = base.GetClientIp(request).ToString(),
				Endpoint = request.RequestUri.AbsolutePath.ToLowerInvariant()
			};
		}



		protected override HttpResponseMessage QuotaExceededResponse(HttpRequestMessage request, object content, HttpStatusCode responseCode, string retryAfter)
		{
			var response = request.CreateResponse<ThrottleResponse>(responseCode, new ThrottleResponse { Success = false, Error = (content ?? string.Empty).ToString() });
			response.Headers.Add("Retry-After", new string[] { retryAfter });
			return response;
		}
	}
}