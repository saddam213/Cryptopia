using Cryptopia.Infrastructure.Common.DataTables;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using System.Web.Mvc;
using Cryptopia.Common.Deposit;
using Web.Site.Extensions;
using Cryptopia.Common.Referral;

namespace Web.Site.Controllers
{
	public class AdminReferralController : BaseController
	{
		public IReferralReader ReferralReader { get; set; }

		[HttpGet]
		[AuthorizeAjax(Roles = "Admin, Moderator")]
		public ActionResult GetReferrals()
		{
			return PartialView("_Referral");
		}

		[HttpPost]
		[AuthorizeAjax(Roles = "Admin, Moderator")]
		public async Task<ActionResult> GetReferrals(DataTablesModel model)
		{
			return DataTable(await ReferralReader.AdminGetHistory(model));
		}
	}
}