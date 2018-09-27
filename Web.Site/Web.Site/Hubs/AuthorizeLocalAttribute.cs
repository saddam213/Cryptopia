using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web.Site.Hubs
{
	public class AuthorizeLocalAttribute : AuthorizeAttribute
	{
		private static readonly string _authToken = ConfigurationManager.AppSettings["Signalr_AuthToken"];

		public override bool AuthorizeHubMethodInvocation(IHubIncomingInvokerContext hubIncomingInvokerContext, bool appliesToMethod)
		{
			if (hubIncomingInvokerContext.Hub != null && hubIncomingInvokerContext.Hub.Context != null && hubIncomingInvokerContext.Hub.Context.Headers != null)
				return _authToken == hubIncomingInvokerContext.Hub.Context.Headers.Get("auth");

			return false;
		}
	}
}
