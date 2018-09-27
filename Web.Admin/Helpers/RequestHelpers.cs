using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace Web.Admin.Helpers
{
	public static class RequestHelpers
	{
		public static string GetIPAddress(this HttpRequestBase request)
		{
			try
			{
				string szRemoteAddr = request.UserHostAddress;
				string szXForwardedFor = request.ServerVariables["X_FORWARDED_FOR"];
				string szIP = "0.0.0.0";

				if (szXForwardedFor == null)
				{
					szIP = szRemoteAddr;
				}
				else
				{
					szIP = szXForwardedFor;
					if (szIP.IndexOf(",") > 0)
					{
						string[] arIPs = szIP.Split(',');

						foreach (string item in arIPs)
						{
							if (!IsLocalHost(item))
							{
								return item;
							}
						}
					}
				}
				return szIP;
			}
			catch (Exception)
			{
				return string.Empty;
			}
		}

		private static bool IsLocalHost(string input)
		{
			IPAddress[] host;
			//get host addresses
			try
			{
				host = Dns.GetHostAddresses(input);
			}
			catch (Exception)
			{
				return false;
			}
			//get local adresses
			IPAddress[] local = Dns.GetHostAddresses(Dns.GetHostName());
			//check if local
			return host.Any(hostAddress => IPAddress.IsLoopback(hostAddress) || local.Contains(hostAddress));
		}
	}
}