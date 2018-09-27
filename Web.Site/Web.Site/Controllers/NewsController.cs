using Cryptopia.Common.News;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Web.Site.Controllers
{
	public class NewsController : BaseController
	{
		public INewsReader NewsReader { get; set; }

		[HttpGet]
		[AllowAnonymous]
		public async Task<ActionResult> Index()
		{
			return View(await NewsReader.GetNews());
		}
	}
}