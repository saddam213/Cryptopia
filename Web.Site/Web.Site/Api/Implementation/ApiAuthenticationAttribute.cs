using Cryptopia.Cache;
using Cryptopia.Common.User;
using System;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Caching;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Filters;
using System.Web.Http.Results;

namespace Web.Site.Api.Authentication
{
	public class ApiAuthenticationAttribute : Attribute, IAuthenticationFilter
	{
		private static bool _useRedisCache = bool.Parse(ConfigurationManager.AppSettings["Redis_ApiReplayCache_Enabled"]);
		private static DistributedCache _distributedCache = new DistributedCache(RedisConnectionFactory.GetApiReplayCacheConnection(), _useRedisCache);
		private readonly int _requestMaxAge = 5;
		private readonly string _authenticationScheme = "amx";

		public ApiAuthenticationAttribute()
		{
		}

		public async Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
		{
			await context.Request.Content.LoadIntoBufferAsync().ConfigureAwait(false);
			var request = context.Request;
			if (request.Headers.Authorization != null && _authenticationScheme.Equals(request.Headers.Authorization.Scheme, StringComparison.OrdinalIgnoreCase))
			{
				var rawAuthzHeader = request.Headers.Authorization.Parameter;
				var autherizationHeaderArray = GetAutherizationHeaderValues(rawAuthzHeader);
				if (autherizationHeaderArray != null)
				{
					var apiKey = autherizationHeaderArray[0];
					var incomingBase64Signature = autherizationHeaderArray[1];
					var nonce = autherizationHeaderArray[2];

					var apiAuthKey = await ApiKeyStore.GetApiAuthKey(apiKey);
					if (apiAuthKey != null && apiAuthKey.IsEnabled)
					{
						var validResponse = await IsValidRequest(request, apiAuthKey, incomingBase64Signature, nonce);
						if (string.IsNullOrEmpty(validResponse))
						{
							var currentPrincipal = new GenericPrincipal(new GenericIdentity(apiAuthKey.UserId), null);
							context.Principal = currentPrincipal;
						}
						else
						{
							context.ErrorResult = CreateErrorResponse(context.Request, validResponse);
						}
					}
					else
					{
						context.ErrorResult = new UnauthorizedResult(new AuthenticationHeaderValue[0], context.Request);
					}
				}
				else
				{
					context.ErrorResult = CreateErrorResponse(context.Request, "Invalid authorization header content.");
				}
			}
			else
			{
				context.ErrorResult = CreateErrorResponse(context.Request, "Invalid authorization header.");
			}
		}

		public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
		{
			context.Result = new ApiResultWithChallenge(context.Result);
			return Task.FromResult(0);
		}

		public bool AllowMultiple
		{
			get { return false; }
		}

		private string[] GetAutherizationHeaderValues(string rawAuthHeader)
		{
			var credArray = rawAuthHeader.Split(':');
			if (credArray.Length == 3)
			{
				return credArray;
			}
			return null;
		}

		private async Task<string> IsValidRequest(HttpRequestMessage request, UserApiAuthKey apiAuthKey, string incomingBase64Signature, string nonce)
		{
			if (!apiAuthKey.IsValid)
				return $"Authentication key is invalid.";

			if (await IsReplayRequest(apiAuthKey.Key, nonce))
				return $"Nonce has already been used for this request.";

			string requestHttpMethod = request.Method.Method;
			var content = await request.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
			string requestContentBase64String = GetMD5String(content);
			string requestContentBase64StringNonMd5 = Convert.ToBase64String(content);
			string requestUri = HttpUtility.UrlEncode(request.RequestUri.AbsoluteUri.ToLower());
			string base64Signature = String.Format("{0}{1}{2}{3}{4}", apiAuthKey.Key, requestHttpMethod, requestUri, nonce, requestContentBase64String);
			string base64SignatureNonMd5 = String.Format("{0}{1}{2}{3}{4}", apiAuthKey.Key, requestHttpMethod, requestUri, nonce, requestContentBase64StringNonMd5);
			if (VerifySignature(apiAuthKey, incomingBase64Signature, base64Signature) || VerifySignature(apiAuthKey, incomingBase64Signature, base64SignatureNonMd5))
			{
				return string.Empty;
			}
			return $"Signature does not match request parameters.";
		}

		private bool VerifySignature(UserApiAuthKey apiAuthKey, string requestSignature, string serverSignature)
		{
			var secretKeyBytes = Convert.FromBase64String(apiAuthKey.Secret);
			byte[] signature = Encoding.UTF8.GetBytes(serverSignature);
			using (HMACSHA256 hmac = new HMACSHA256(secretKeyBytes))
			{
				byte[] signatureBytes = hmac.ComputeHash(signature);
				if (requestSignature.Equals(Convert.ToBase64String(signatureBytes), StringComparison.Ordinal))
				{
					return true;
				}
				return false;
			}
		}

		private string GetMD5String(byte[] content)
		{
			byte[] hash = ComputeHash(content);
			if (hash != null)
			{
				return Convert.ToBase64String(hash);
			}
			return string.Empty;
		}

		private async Task<bool> IsReplayRequest(string key, string nonce)
		{
			return !await _distributedCache.AddIfNotExistsAsync(key + nonce, string.Empty, TimeSpan.FromMinutes(_requestMaxAge));
		}

		private static byte[] ComputeHash(byte[] content)
		{
			using (var md5 = MD5.Create())
			{
				byte[] hash = null;
				if (content.Length != 0)
				{
					hash = md5.ComputeHash(content);
				}
				return hash;
			}
		}

		private JsonResult<ApiErrorResponse> CreateErrorResponse(HttpRequestMessage request, string error)
		{
			return new JsonResult<ApiErrorResponse>(new ApiErrorResponse(error), new Newtonsoft.Json.JsonSerializerSettings() { }, Encoding.UTF8, request);
		}
	}
}
