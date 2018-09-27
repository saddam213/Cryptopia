using Microsoft.AspNet.SignalR;
using System;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.SignalR.Hubs;

namespace Web.Site.Notifications
{
	public class HubIdentityProvider : IUserIdProvider
	{
		public string GetUserId(IRequest request)
		{
			if (request.User != null && request.User.Identity.IsAuthenticated)
			{
				return request.User.Identity.GetUserId();
			}
			return Guid.Empty.ToString();
		}
	}
}