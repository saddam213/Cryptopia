using Cryptopia.Admin.Common.Nzdt;
using Cryptopia.Infrastructure.Common.DataTables;
using Cryptopia.Infrastructure.Common.Results;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Web.Admin.Helpers;

namespace Web.Admin.Controllers
{
	[Authorize(Roles = "BankAdmin")]
	public class NzdtController : BaseController
	{
		public INzdtImportService NzdtImportService { get; set; }
		public INzdtReader NzdtReader { get; set; }
		public INzdtWriter NzdtWriter { get; set; }

		public ActionResult Index()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Upload(HttpPostedFileBase upload)
		{
			if (ModelState.IsValid)
			{
				if (upload != null && upload.ContentLength > 0)
				{
					var result = await NzdtImportService.ValidateAndUpload(User.Identity.GetUserId(), upload.InputStream);

					if (result.Success)
					{
						return View("UploadResult", result.Result);
					}
					else
					{
						ModelState.AddModelError("File", result.Message);
						return View("Index");
					}
				}
				else
				{
					ModelState.AddModelError("File", "Please Upload Your file");
					return View("Index");
				}
			}

			return View("Index");
		}

		[HttpGet]
		public async Task<ActionResult> UpdateErroredTransaction(int id)
		{
			var model = await NzdtReader.GetUpdateTransaction(id);

			if (model == null)
			{
				return View("NoTransactionModal");
			}

			return View("UpdateErroredTransactionModal", model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> AddUserToErroredTransaction(UpdateNzdtTransactionModel model)
		{
			if (!ModelState.IsValid)
			{
				return View("UpdateErroredTransactionModal", model);
			}

			var result = await NzdtWriter.AddUserToTransaction(User.Identity.GetUserId(), model);

			if (!ModelState.IsWriterResultValid(result))
			{
				return View("UpdateErroredTransactionModal", model);
			}

			return CloseModalSuccess(result.Message);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ReprocessNotVerifiedTransaction(UpdateNzdtTransactionModel model)
		{
			if (!ModelState.IsValid)
			{
				return View("UpdateErroredTransactionModal", model);
			}

			var result = await NzdtWriter.ReprocessNotVerifiedTransaction(User.Identity.GetUserId(), model);

			if (!ModelState.IsWriterResultValid(result))
			{
				return View("UpdateErroredTransactionModal", model);
			}

			return CloseModalSuccess(result.Message);
		}

		[HttpPost]
		public async Task<ActionResult> GetAllTransations(DataTablesModel model)
		{
			return DataTable(await NzdtReader.GetAllTransactions(model));
		}

		[HttpPost]
		public async Task<ActionResult> GetErroredTransactions(DataTablesModel model)
		{
			return DataTable(await NzdtReader.GetErroredTransactions(model));
		}

		[HttpPost]
		public async Task<ActionResult> GetReadyTransactions(DataTablesModel model)
		{
			return DataTable(await NzdtReader.GetReadyTransactions(model));
		}

		[HttpPost]
		public async Task<ActionResult> GetProcessedTransactions(DataTablesModel model)
		{
			return DataTable(await NzdtReader.GetProcessedTransactions(model));
		}
	}
}
