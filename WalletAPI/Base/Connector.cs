using System;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Cryptopia.WalletAPI.Base
{
	/// <summary>
	/// Class for handling connections to an RPC client
	/// </summary>
	public class Connector
	{
		/// <summary>
		/// The _server ip
		/// </summary>
		private readonly string _serverIp;

		/// <summary>
		/// The _username
		/// </summary>
		private readonly string _username;

		/// <summary>
		/// The _password
		/// </summary>
		private readonly string _password;

		/// <summary>
		/// The web request timeout
		/// </summary>
		private readonly int _timeout = 0;

		/// <summary>
		/// Initializes a new instance of the <see cref="Connector"/> class.
		/// </summary>
		/// <param name="ip">The ip.</param>
		/// <param name="port">The port.</param>
		/// <param name="username">The username.</param>
		/// <param name="password">The password.</param>
		/// <exception cref="System.ArgumentException">
		/// Invalid IP
		/// or
		/// Invalid Port
		/// or
		/// Invalid Username
		/// or
		/// Invalid Password
		/// </exception>
		protected Connector(string ip, int port, string username, string password, int timeout = 120000)
		{
			if (string.IsNullOrEmpty(ip))
			{
				throw new ArgumentException("Invalid IP");
			}
			if (port < 0)
			{
				throw new ArgumentException("Invalid Port");
			}
			if (string.IsNullOrEmpty(username))
			{
				throw new ArgumentException("Invalid Username");
			}
			if (string.IsNullOrEmpty(password))
			{
				throw new ArgumentException("Invalid Password");
			}

			_timeout = timeout;
			_serverIp = string.Format("http://{0}:{1}", ip, port);
			_username = username;
			_password = password;
		}

		/// <summary>
		/// Executes the wallet function.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="functionType">Name of the method.</param>
		/// <param name="parameters">The parameters.</param>
		/// <returns>the function result</returns>
		protected T ExecuteWalletFunction<T>(WalletFunctionType functionType, params object[] parameters)
		{
			var rawResult = ExecuteWalletRpcFunction(functionType, parameters);
			if (rawResult == null) return default(T);
			var json = rawResult["result"].ToString();
			if (string.IsNullOrEmpty(json)) return default(T);
			if (typeof (T) == typeof (string))
			{
				return (T) (object) json;
			}

			if (typeof (T) == typeof (bool))
			{
				return (T) (object) bool.Parse(json);
			}

			return JsonConvert.DeserializeObject<T>(json);
		}

		/// <summary>
		/// Executes the wallet RPC function.
		/// </summary>
		/// <param name="functionName">Name of the function.</param>
		/// <param name="parameters">The parameters.</param>
		/// <returns>the json object response of the call</returns>
		private JObject ExecuteWalletRpcFunction(WalletFunctionType functionName, params object[] parameters)
		{
			var webRequest = CreateWebRequest();
			var jsonRequest = new JObject
			{
				new JProperty("jsonrpc", "1.0"),
				new JProperty("id", "1"),
				new JProperty("method", functionName.ToString().ToLower()),
				new JProperty("params", new JArray(parameters))
			};

			var s = JsonConvert.SerializeObject(jsonRequest);
			var byteArray = Encoding.UTF8.GetBytes(s);
			webRequest.ContentLength = byteArray.Length;
			using (var dataStream = webRequest.GetRequestStream())
			{
				dataStream.Write(byteArray, 0, byteArray.Length);
			}

			using (var webResponse = webRequest.GetResponse())
			{
				using (var streamReader = new StreamReader(webResponse.GetResponseStream(), true))
				{
					var jsonData = streamReader.ReadToEnd();
					try
					{
						return (JObject) JsonConvert.DeserializeObject(jsonData);
					}
					catch
					{
					}
					return JObject.Parse(jsonData);
				}
			}
		}

		/// <summary>
		/// Creates the web request.
		/// </summary>
		/// <returns></returns>
		private HttpWebRequest CreateWebRequest()
		{
			var webRequest = (HttpWebRequest) WebRequest.Create(_serverIp);
			if (_timeout > 0)
			{
				webRequest.Timeout = _timeout;
			}
			webRequest.Credentials = new NetworkCredential(_username, _password);
			webRequest.ContentType = "application/json-rpc";
			webRequest.Method = "POST";

			string userpass = string.Format("{0}:{1}", _username, _password);
			string encoded = Convert.ToBase64String(Encoding.ASCII.GetBytes(userpass));
			string authorizationHearder = string.Format("{0} {1}", "Basic", encoded);
			webRequest.Headers.Add(HttpRequestHeader.Authorization, authorizationHearder);
			return webRequest;
		}
	}
}