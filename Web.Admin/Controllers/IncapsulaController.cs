using System.Web.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Cryptopia.Admin.Common.Incapsula;
using Cryptopia.Infrastructure.Helpers;
using Cryptopia.Enums;

namespace Web.Admin.Controllers
{
	[Authorize(Roles = "Admin")]
	public class IncapsulaController :  BaseController
    {
        public IIncapsulaReader Reader { get; set; }
        public IIncapsulaWriter Writer { get; set; }

        // GET: Incapsula
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> RefreshVisitStatistics()
        {
            var result = await Reader.GetSiteVisitsReport();
            return Json(result);
        }

        [HttpPost]
        public async Task<ActionResult> RefreshHitStatistics()
        {
            var result = await Reader.GetSiteHitsReport();
            return Json(result);
        }

        [HttpPost]
        public async Task<ActionResult> RefreshThreatStatistics()
        {
            var result = await Reader.GetThreatStatisticsReport();
            return Json(result);
        }

        [HttpPost]
        public async Task<ActionResult> RefreshVisitDistribution()
        {
            var result = await Reader.GetVisitsByCountryReport();
            return Json(result);
        }

        [HttpPost]
        public async Task<ActionResult> RefreshGeoDistribution()
        {
            var result = await Reader.GetGeoDistribution();
            return Json(result);
        }

        [HttpGet]
        public async Task<ActionResult> BlockIpAddress()
        {
            return await Task.FromResult(View("BlockIpAddressModal", new BlockIpAddressModel()));
        }

        [HttpPost]
        public async Task<ActionResult> BlockIpAddress(BlockIpAddressModel model)
        {
            if (!ModelState.IsValid)
                return View("BlockIpAddressModal", model);

            if (!await CryptopiaAuthenticationHelper.VerifyTwoFactorCode(AuthenticatedFeatureType.BlacklistIP, model.AuthenticationCode))
            {
                ModelState.AddModelError("", "Two factor token incorrect.");
                return View("BlockIpAddressModal", model);
            }

            await Writer.BlacklistIpAddress(User.Identity.GetUserId(), model.Address);

            return CloseModalSuccess($"Ip Address {model.Address} successfully blocked.");
        }
    }
}