using Cryptopia.Admin.Common.Approval;
using Cryptopia.Admin.Common.Reprocessing;
using Cryptopia.Infrastructure.Common.DataTables;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using System.Web.Mvc;
using Web.Admin.Helpers;
using Web.Admin.Models;

namespace Web.Admin.Controllers
{
	[Authorize(Roles = "Admin")]
	public class ReprocessingController : BaseController
    {
        public IReprocessingReader ReprocessingReader { get; set; }
        public IReprocessingWriter ReprocessingWriter { get; set; }

        // GET: SomethingNew
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> GetIncompleteWithdrawals(DataTablesModel model)
        {
            return DataTable(await ReprocessingReader.GetIncompleteWithdrawals(model));
        }

        [HttpPost]
        public async Task<ActionResult> GetWalletTransactions(WalletTxRequestModel model, DataTablesModel tableModel)
        {
            DataTablesResponse response = await ReprocessingReader.GetWalletTransactions(model, tableModel);
            return DataTable(response);
        }

        [HttpGet]
        public async Task<ActionResult> UpdateWithdrawalTxId(int id)
        {
            System.Console.WriteLine("UpdateWithdrawalTxId entered. Id: " + id);

            var model = await ReprocessingReader.GetWithdrawalToUpdate(id);

            if (model == null)
                return ViewMessageModal(new ViewMessageModel(Models.ViewMessageType.Danger, "Error", $"Withdrawal '{id}' not found"));

            if (model.RetryCount > 0)
                return ViewMessageModal(new ViewMessageModel(Models.ViewMessageType.Danger, "Error", $"Unable to reprocess Withdrawal '{id}'. Retry count limit reached."));

            if (model.Status != Cryptopia.Enums.WithdrawStatus.Processing)
                return ViewMessageModal(new ViewMessageModel(Models.ViewMessageType.Danger, "Error", $"Withdrawal '{id}' not in correct state to reprocess. Current state: '{model.Status}'"));

            return View("UpdateWithdrawalTxIdModal", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateWithdrawalTxId(UpdateWithdrawalTxModel model)
        {
            if (!ModelState.IsValid)
                return CloseModalError("Invalid input.");

            var result = await ReprocessingWriter.UpdateWithrawalTransactionId(User.Identity.GetUserId(), model);

            if (!ModelState.IsWriterResultValid(result))
                return CloseModalError(result.Message);

            return CloseModalSuccess(result.Message);
        }

        [HttpGet]
        public async Task<ActionResult> GetPageOptions(int blockLength)
        {
            var model = new ReprocessingOptionsModel();
            model.WalletSearchBlockLength = blockLength;
            return await Task.FromResult(View("ReprocessingOptionsModal", model));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdatePageOptions(ReprocessingOptionsModel model)
        {
            if (!ModelState.IsValid)
                return CloseModalError("Invalid input.");

            if (model.WalletSearchBlockLength < 0)
                return CloseModalError("Invalid input. Must be greater than 0.");

            return await Task.FromResult(CloseModal(new { Success = true, WalletBlockLength = model.WalletSearchBlockLength }));
        }

        [HttpPost]
        public async Task<ActionResult> GetPendingApprovals(DataTablesModel model)
        {
            DataTablesResponse response = await ReprocessingReader.GetPendingApprovals(model);
            return DataTable(response);
        }

        [HttpGet]
        public async Task<ActionResult> ApproveWithdrawalReprocessing(int id)
        {
            var model = await ReprocessingReader.GetApproval(id);
            return View("WithdrawalReprocessingApproveModal", model);
        }

        [HttpPost]
        public async Task<ActionResult> ApproveWithdrawalReprocessing(ReprocessingApprovalsModel model)
        {
            if (!ModelState.IsValid)
                return CloseModalError("Invalid input.");

            var userId = User.Identity.GetUserId();
            var result = await ReprocessingWriter.ApproveWithdrawalReprocessing(userId, model);
            if (!ModelState.IsWriterResultValid(result))
                return CloseModalError(result.Message);

            return CloseModalSuccess("Approval confirmed");
        }
    }
}