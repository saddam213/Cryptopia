using System.Web.Mvc;
using System.Web.Routing;

namespace Web.Site
{
	public class RouteConfig
	{
		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			// admin
			//routes.MapRoute(
			//			   name: "AdminAction",
			//			   url: "AdminAction/{action}/{id}",
			//			   defaults: new { controller = "Admin", id = UrlParameter.Optional });

			// Home
			//routes.MapRoute(
			//	name: "HomeAction",
			//	url: "HomeAction/{action}/{id}",
			//	defaults: new {controller = "Home", id = UrlParameter.Optional});

			//routes.MapRoute(
			//	name: "Home",
			//	url: "Home/{action}",
			//	defaults: new {controller = "Home"});


			routes.MapRoute(
				name: "Chat",
				url: "Chat",
				defaults: new { controller = "Chat", action = "Index" });

			// Market
			routes.MapRoute(
				name: "MarketPlaceAction",
				url: "MarketPlaceAction/{action}/{id}",
				defaults: new { controller = "MarketPlace", id = UrlParameter.Optional });

			routes.MapRoute(
				name: "MarketPlace",
				url: "MarketPlace",
				defaults: new { controller = "MarketPlace", action = "Index" });

			routes.MapRoute(
				name: "MarketItem",
				url: "MarketItem/{marketItemId}",
				defaults: new { controller = "MarketPlace", action = "MarketItem", marketItemId = -1 });

			routes.MapRoute(
				name: "MarketItemDetail",
				url: "MarketItemDetail/{id}",
				defaults: new { controller = "MarketPlace", action = "MarketItemDetail", id = -1 });
			routes.MapRoute(
				name: "MarketFeedback",
				url: "MarketFeedback/{username}",
				defaults: new { controller = "MarketPlace", action = "MarketFeedback", username = string.Empty });

			// User Section
			routes.MapRoute(name: "User", url: "User", defaults: new { controller = "User", action = "Index", id = "Account" });
			routes.MapRoute(name: "Account", url: "Account", defaults: new { controller = "User", action = "Index", id = "Account" });
			routes.MapRoute(name: "Shareholder", url: "Shareholder", defaults: new { controller = "User", action = "Index", id = "Shareholder" });
			routes.MapRoute(name: "Settings", url: "Settings", defaults: new { controller = "User", action = "Index", id = "Settings" });
			routes.MapRoute(name: "Security", url: "Security", defaults: new { controller = "User", action = "Index", id = "Security" });
			routes.MapRoute(name: "Messages", url: "Messages", defaults: new { controller = "User", action = "Index", id = "Messages" });
			routes.MapRoute(name: "Notifications", url: "Notifications", defaults: new { controller = "User", action = "Index", id = "Notifications" });
			routes.MapRoute(name: "Karma", url: "Karma", defaults: new { controller = "User", action = "Index", id = "Karma" });
			routes.MapRoute(name: "Balances", url: "Balances", defaults: new { controller = "User", action = "Index", id = "Balances" });
			routes.MapRoute(name: "Trades", url: "Trades", defaults: new { controller = "User", action = "Index", id = "Trades" });
			routes.MapRoute(name: "Miners", url: "Miners", defaults: new { controller = "User", action = "Index", id = "Miners" });
			routes.MapRoute(name: "MarketItems", url: "MarketItems", defaults: new { controller = "User", action = "Index", id = "MarketItems" });
			routes.MapRoute(name: "DepositHistory", url: "DepositHistory", defaults: new { controller = "User", action = "Index", id = "DepositHistory" });
			routes.MapRoute(name: "WithdrawHistory", url: "WithdrawHistory", defaults: new { controller = "User", action = "Index", id = "WithdrawHistory" });
			routes.MapRoute(name: "TransferHistory", url: "TransferHistory", defaults: new { controller = "User", action = "Index", id = "TransferHistory" });
			routes.MapRoute(name: "TradeHistory", url: "TradeHistory", defaults: new { controller = "User", action = "Index", id = "TradeHistory" });
			routes.MapRoute(name: "MarketplaceHistory", url: "MarketplaceHistory", defaults: new { controller = "User", action = "Index", id = "MarketplaceHistory" });
			routes.MapRoute(name: "MineshaftHistory", url: "MineshaftHistory", defaults: new { controller = "User", action = "Index", id = "MineshaftHistory" });
			routes.MapRoute(name: "TermDepositStatus", url: "TermDepositStatus", defaults: new { controller = "User", action = "Index", id = "TermDepositStatus" });
			routes.MapRoute(name: "PaytopiaHistory", url: "PaytopiaHistory", defaults: new { controller = "User", action = "Index", id = "PaytopiaHistory" });
			routes.MapRoute(name: "ReferralHistory", url: "ReferralHistory", defaults: new { controller = "User", action = "Index", id = "ReferralHistory" });

			// Admin Section
			routes.MapRoute(name: "Admin", url: "Admin", defaults: new { controller = "Admin", action = "Index", id = "AdminSettings" });
			routes.MapRoute(name: "AdminSettings", url: "AdminSettings", defaults: new { controller = "Admin", action = "Index", id = "AdminSettings" });
			routes.MapRoute(name: "AdminSupport", url: "AdminSupport", defaults: new { controller = "Admin", action = "Index", id = "AdminSettings" });
			routes.MapRoute(name: "AdminUser", url: "AdminUser", defaults: new { controller = "Admin", action = "Index", id = "AdminUser" });
			routes.MapRoute(name: "AdminSecurity", url: "AdminSecurity", defaults: new { controller = "Admin", action = "Index", id = "AdminSecurity" });
			routes.MapRoute(name: "AdminDeposit", url: "AdminDeposit", defaults: new { controller = "Admin", action = "Index", id = "AdminDeposit" });
			routes.MapRoute(name: "AdminWithdraw", url: "AdminWithdraw", defaults: new { controller = "Admin", action = "Index", id = "AdminWithdraw" });
			routes.MapRoute(name: "AdminTransfer", url: "AdminTransfer", defaults: new { controller = "Admin", action = "Index", id = "AdminTransfer" });
			routes.MapRoute(name: "AdminChat", url: "AdminChat", defaults: new { controller = "Admin", action = "Index", id = "AdminChat" });
			routes.MapRoute(name: "AdminEmoticon", url: "AdminEmoticon", defaults: new { controller = "Admin", action = "Index", id = "AdminEmoticon" });
			routes.MapRoute(name: "AdminLotto", url: "AdminLotto", defaults: new { controller = "Admin", action = "Index", id = "AdminLotto" });
			routes.MapRoute(name: "AdminTermDeposit", url: "AdminTermDeposit", defaults: new { controller = "Admin", action = "Index", id = "AdminTermDeposit" });
			routes.MapRoute(name: "AdminPoolWorker", url: "AdminPoolWorker", defaults: new { controller = "Admin", action = "Index", id = "AdminPoolWorker" });
			routes.MapRoute(name: "AdminMineshaft", url: "AdminMineshaft", defaults: new { controller = "Admin", action = "Index", id = "AdminMineshaft" });
			routes.MapRoute(name: "AdminPaytopia", url: "AdminPaytopia", defaults: new { controller = "Admin", action = "Index", id = "AdminPaytopia" });
			routes.MapRoute(name: "AdminCurrency", url: "AdminCurrency", defaults: new { controller = "Admin", action = "Index", id = "AdminCurrency" });
			routes.MapRoute(name: "AdminTradePair", url: "AdminTradePair", defaults: new { controller = "Admin", action = "Index", id = "AdminTradePair" });
			routes.MapRoute(name: "AdminMineshaftPayment", url: "AdminMineshaftPayment", defaults: new { controller = "Admin", action = "Index", id = "AdminMineshaftPayment" });
			routes.MapRoute(name: "AdminNews", url: "AdminNews", defaults: new { controller = "Admin", action = "Index", id = "AdminNews" });
			routes.MapRoute(name: "AdminReferral", url: "AdminReferral", defaults: new { controller = "Admin", action = "Index", id = "AdminReferral" });

			// Account
			routes.MapRoute(name: "Login", url: "Login", defaults: new { controller = "Login", action = "Login" });
			routes.MapRoute(name: "Register", url: "Register", defaults: new { controller = "Login", action = "Register" });
			routes.MapRoute(name: "LogOff", url: "LogOff", defaults: new { controller = "Login", action = "LogOff" });


			// Support
			routes.MapRoute(name: "Support", url: "Support", defaults: new { controller = "Support", action = "Support" });
			routes.MapRoute(name: "UserSupport", url: "UserSupport", defaults: new { controller = "Support", action = "UserSupport" });

			routes.MapRoute(
				name: "Default",
				url: "{controller}/{action}/{id}",
				defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional });
		}
	}
}