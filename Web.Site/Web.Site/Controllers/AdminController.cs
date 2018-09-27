using System.Web.Mvc;

namespace Web.Site.Controllers
{
	[Authorize(Roles = "Admin, Moderator")]
	public class AdminController : BaseController
	{
		[HttpGet]
		[Authorize(Roles = "Admin, Moderator")]
		public ActionResult Index(string id)
		{
			ViewBag.Section = id;
			return View();
		}
	}
}