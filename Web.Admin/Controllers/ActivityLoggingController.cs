using Cryptopia.Admin.Common.ActivityLogging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Web.Admin.Controllers
{
	[Authorize(Roles = "SuperUser")]
	public class ActivityLoggingController : BaseController
    {
        public IActivityLogReader ActivityReader { get; set; }

        // GET: ActivityLogging
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> GetSupportTicketStats()
        {
            return Json(await ActivityReader.GetSupportTicketStats());
        }

        [HttpPost]
        public async Task<ActionResult> GetVerificationStats()
        {
            return Json(await ActivityReader.GetVerificationStats());
        }

        [HttpPost]
        public async Task<ActionResult> GetAdminUserActivityStats()
        {
            var data = await ActivityReader.GetAdminUserActivityStats();
            return Json(data);
        }

        [HttpPost]
        public async Task<ActionResult> GetTrendGraphData()
        {
            var data = await ActivityReader.GetActivityTrendGraphData(DateTime.UtcNow.AddDays(-28));
            return Json(data);
        }
    }
}