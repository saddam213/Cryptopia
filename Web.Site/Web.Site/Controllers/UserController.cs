using System.Threading.Tasks;
using System.Web.Mvc;
using Cryptopia.Common.User;
using Microsoft.AspNet.Identity;

namespace Web.Site.Controllers
{
	public class UserController : BaseController
	{
		public IUserProfileReader UserProfileReader { get; set; }

		[HttpGet]
		[Authorize]
		public async Task<ActionResult> Index(string id)
		{
			ViewBag.Section = id;
			var model = await UserProfileReader.GetProfileInfo(User.Identity.GetUserId());
			return View(model);
		}
	}
}