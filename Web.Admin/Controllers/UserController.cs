namespace Web.Admin.Controllers
{
	using System;
	using System.Threading.Tasks;
	using System.Web.Mvc;
	using Cryptopia.Admin.Common.AdminUser;
	using Cryptopia.Admin.Common.Approval;
	using Cryptopia.Common.Deposit;
	using Cryptopia.Common.User;
	using Cryptopia.Infrastructure.Common.DataTables;
	using Microsoft.AspNet.Identity;
	using Web.Admin.Helpers;
	using Web.Admin.Models;

	[Authorize(Roles = "Admin")]
	public class UserController : BaseController
	{
		public IUserReader UserReader { get; set; }
		public IAdminUserReader AdminUserReader { get; set; }
		public IAdminUserWriter AdminUserWriter { get; set; }
		public IAdminApprovalReader AdminApprovalReader { get; set; }
		public IDepositReader DepositReader { get; set; }

		public ActionResult Index(string userName = null)
		{
			ViewBag.UserName = userName;
			return View();
		}

		[HttpPost]
		public async Task<ActionResult> GetUsers(DataTablesModel model)
		{
			var result = DataTable(await UserReader.GetUsers(model));
			return result;
		}

		[HttpPost]
		public async Task<ActionResult> GetUserDetails(string username)
		{
			var data = await AdminUserReader.GetUserDetails(username);
			return Json(data);
		}

		[HttpPost]
		public async Task<ActionResult> GetUserSecurity(string username)
		{
			return Json(await AdminUserReader.GetUserSecurity(username));
		}

		#region User

		[HttpGet]
		public async Task<ActionResult> UpdateUser(string username)
		{
			var model = await AdminUserReader.GetUserUpdate(username);
			if (model == null)
				return
					ViewMessageModal(new ViewMessageModel(ViewMessageType.Warning, "User Not Found!",
						$"Could not find user '{username}'"));

			return View("UpdateUserModal", model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> UpdateUser(AdminUserUpdateModel model)
		{
			if (!ModelState.IsValid)
				return View("UpdateUserModal", model);

			var result = await AdminUserWriter.UpdateUser(model);
			if (!ModelState.IsWriterResultValid(result))
				return View("UpdateUserModal", model);

			return CloseModalSuccess(result.Message);
		}

		#endregion

		#region ChangeEmail

		[HttpGet]
		public async Task<ActionResult> ChangeEmail(string username)
		{
			var model = await AdminUserReader.GetUserDetails(username);
			if (model == null)
				return
					ViewMessageModal(new ViewMessageModel(ViewMessageType.Warning, "User Not Found!",
						$"Could not find user '{username}'"));

			return View("ChangeEmailModal", new AdminChangeEmailModel
			{
				UserName = model.UserName,
				EmailAddress = model.Email
			});
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ChangeEmail(AdminChangeEmailModel model)
		{
			if (!ModelState.IsValid)
				return View("ChangeEmailModal", model);

			var result = await AdminUserWriter.ChangeEmail(User.Identity.GetUserId(), model);
			if (!ModelState.IsWriterResultValid(result))
				return View("ChangeEmailModal", model);

			return CloseModalSuccess(result.Message);
		}

		[HttpGet]
		public async Task<ActionResult> ApproveChangeEmail(int approvalId)
		{
			var model = await AdminApprovalReader.GetApproval(approvalId);
			if (model == null)
				return
					ViewMessageModal(new ViewMessageModel(ViewMessageType.Warning, "Approval Not Found!",
						$"Could not find EmailChange approval #{approvalId}"));

			var user = await UserReader.GetUserByName(model.DataUser);
			return View("ChangeEmailApproveModal", new AdminChangeEmailApproveModel
			{
				Status = model.Status,
				Requestor = model.RequestUser,
				Requested = model.Created,
				Approved = model.Approved,
				Approver = model.ApprovalUser,
				Message = model.Message,
				UserName = model.DataUser,
				NewEmailAddress = model.Data,
				OldEmailAddress = user.Email,
				ApprovalId = approvalId
			});
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ApproveChangeEmail(AdminChangeEmailApproveModel model)
		{
			if (!ModelState.IsValid)
				return View("ChangeEmailApproveModal", model);

			var result = await AdminUserWriter.ApproveChangeEmail(User.Identity.GetUserId(), model);
			if (!ModelState.IsWriterResultValid(result))
				return View("ChangeEmailApproveModal", model);

			return CloseModalSuccess(result.Message);
		}

		#endregion

		[HttpGet]
		public async Task<ActionResult> ActivateUser(string username)
		{
			var model = await AdminUserReader.GetUserDetails(username);
			if (model == null)
				return ViewMessageModal(new ViewMessageModel(ViewMessageType.Warning, "User Not Found!", $"Could not find user '{username}'"));

			return View("ActivateUserModal", new AdminActivateUserModel
			{
				UserName = model.UserName,
				EmailAddress = model.Email,
				IsActivated = model.EmailConfirmed
			});
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ActivateUser(AdminActivateUserModel model)
		{
			if (!ModelState.IsValid)
				return View("ActivateUserModal", model);

			var result = await AdminUserWriter.ActivateUser(User.Identity.GetUserId(), model);
			if (!ModelState.IsWriterResultValid(result))
				return View("ActivateUserModal", model);

			return CloseModalSuccess(result.Message);
		}

		#region Lock/Unlock User
		[HttpPost]
		public async Task<ActionResult> LockUser(string username)
		{
			var result = await AdminUserWriter.LockUser(User.Identity.GetUserId(), username);
			return Json(result);
		}

		[HttpPost]
		public async Task<ActionResult> UnlockUser(string username)
		{
			var result = await AdminUserWriter.UnlockUser(User.Identity.GetUserId(), username);
			return Json(result);
		}
		#endregion

		#region Disable User
		[HttpPost]
		public async Task<ActionResult> DisableUser(string username)
		{
			var result = await AdminUserWriter.DisableUser(User.Identity.GetUserId(), username);
			return Json(result);
		}
		#endregion

		#region Settings

		[HttpGet]
		public async Task<ActionResult> UpdateSettings(string username)
		{
			var model = await AdminUserReader.GetSettingsUpdate(username);
			if (model == null)
				return
					ViewMessageModal(new ViewMessageModel(ViewMessageType.Warning, "User Not Found!",
						$"Could not find user '{username}'"));

			return View("UpdateSettingsModal", model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> UpdateSettings(AdminUserSettingsUpdateModel model)
		{
			if (!ModelState.IsValid)
				return View("UpdateSettingsModal", model);

            var result = await AdminUserWriter.UpdateSettings(User.Identity.GetUserId(), model);
			if (!ModelState.IsWriterResultValid(result))
				return View("UpdateSettingsModal", model);

			//ChatHub.InvalidateUserCache(model.UserId);
			return CloseModalSuccess(result.Message);
		}

		#endregion

		#region TwoFactor

		[HttpGet]
		public async Task<ActionResult> ResetTwoFactor(string username)
		{
			var model = await AdminUserReader.GetUserDetails(username);
			if (model == null)
				return
					ViewMessageModal(new ViewMessageModel(ViewMessageType.Warning, "User Not Found!",
						$"Could not find user '{username}'"));

			return View("ResetTwoFactorModal", new AdminResetTwoFactorModel
			{
				UserName = model.UserName
			});
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ResetTwoFactor(AdminResetTwoFactorModel model)
		{
			if (!ModelState.IsValid)
				return View("ResetTwoFactorModal", model);

			var result = await AdminUserWriter.ResetTwoFactor(User.Identity.GetUserId(), model);
			if (!ModelState.IsWriterResultValid(result))
				return View("ResetTwoFactorModal", model);

			return CloseModalSuccess(result.Message);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ResetAllTwoFactor(AdminResetTwoFactorModel model)
		{
			if (!ModelState.IsValid)
				return View("ResetTwoFactorModal", model);

			var result = await AdminUserWriter.ResetAllTwoFactor(User.Identity.GetUserId(), model);
			if (!ModelState.IsWriterResultValid(result))
				return View("ResetTwoFactorModal", model);

			return CloseModalSuccess(result.Message);
		}

		[HttpGet]
		public async Task<ActionResult> ApproveResetAllTwoFactor(int approvalId)
		{
			var model = await AdminApprovalReader.GetApproval(approvalId);

			if (model == null || !(
				model.Type == Cryptopia.Enums.ApprovalQueueType.ResetTwoFactor
				|| model.Type == Cryptopia.Enums.ApprovalQueueType.ResetAllTwoFactor))
			{
				return ViewMessageModal(new ViewMessageModel(ViewMessageType.Warning, "Approval Not Found!",
					$"Could not find ResetTwoFactor approval #{approvalId}"));
			}

			return View("ResetTwoFactorApproveModal", new AdminResetTwoFactorApproveModel
			{
				Requestor = model.RequestUser,
				Status = model.Status,
				UserName = model.DataUser,
				Type = model.Data,
				ApprovalId = model.Id,
				Requested = model.Created,
				Message = model.Message
			});
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ApproveResetTwoFactor(AdminResetTwoFactorApproveModel model)
		{
			if (!ModelState.IsValid)
				return View("ResetTwoFactorApproveModal", model);

			var result = await AdminUserWriter.ApproveResetAllTwoFactor(User.Identity.GetUserId(), model);
			if (!ModelState.IsWriterResultValid(result))
				return View("ResetTwoFactorApproveModal", model);

			return CloseModalSuccess(result.Message);
		}

		#endregion

		#region Api

		[HttpGet]
		public async Task<ActionResult> UpdateApi(string username)
		{
			var model = await AdminUserReader.GetApiUpdate(username);
			if (model == null)
				return
					ViewMessageModal(new ViewMessageModel(ViewMessageType.Warning, "User Not Found!",
						$"Could not find user '{username}'"));

			return View("UpdateApiModal", model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> UpdateApi(AdminUserApiUpdateModel model)
		{
			if (!ModelState.IsValid)
				return View("UpdateApiModal", model);

            var result = await AdminUserWriter.UpdateApi(User.Identity.GetUserId(), model);
			if (!ModelState.IsWriterResultValid(result))
				return View("UpdateApiModal", model);

			//ChatHub.InvalidateUserCache(model.UserId);
			return CloseModalSuccess(result.Message);
		}

		#endregion

		#region Profile

		[HttpGet]
		public async Task<ActionResult> UpdateProfile(string username)
		{
			var model = await AdminUserReader.GetProfileUpdate(username);
			if (model == null)
				return
					ViewMessageModal(new ViewMessageModel(ViewMessageType.Warning, "User Not Found!",
						$"Could not find user '{username}'"));

			return View("UpdateProfileModal", model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> UpdateProfile(AdminUserProfileUpdateModel model)
		{
			if (!ModelState.IsValid)
				return View("UpdateProfileModal", model);

			var result = await AdminUserWriter.UpdateProfile(model);
			if (!ModelState.IsWriterResultValid(result))
				return View("UpdateProfileModal", model);

			//ChatHub.InvalidateUserCache(model.UserId);
			return CloseModalSuccess(result.Message);
		}

		#endregion

		[HttpPost]
		public async Task<ActionResult> GetDeposits(string username, DataTablesModel model)
		{
			return DataTable(await AdminUserReader.GetUserDeposits(username, model));
		}

		[HttpPost]
		public async Task<ActionResult> GetWithdrawals(string username, DataTablesModel model)
		{
			return DataTable(await AdminUserReader.GetUserWithdraw(username, model));
		}

		[HttpPost]
		public async Task<ActionResult> GetTransfers(string username, DataTablesModel model)
		{
			return DataTable(await AdminUserReader.GetUserTransfer(username, model));
		}

		[HttpPost]
		public async Task<ActionResult> GetTrades(string username, DataTablesModel model)
		{
			return DataTable(await AdminUserReader.GetUserTrades(username, model));
		}

		[HttpPost]
		public async Task<ActionResult> GetTradeHistory(string username, DataTablesModel model)
		{
			return DataTable(await AdminUserReader.GetUserTradeHistory(username, model));
		}

		[HttpPost]
		public async Task<ActionResult> GetOpenTickets(string username, DataTablesModel model)
		{
			return DataTable(await AdminUserReader.GetOpenTickets(username, model));
		}

		[HttpPost]
		public async Task<ActionResult> GetBalances(string username, DataTablesModel model)
		{
			return DataTable(await AdminUserReader.GetUserBalances(username, model));
		}

		[HttpPost]
		public async Task<ActionResult> GetAddresses(string username, DataTablesModel model)
		{
			return DataTable(await AdminUserReader.GetUserAddresses(username, model));
		}

		[HttpPost]
		public async Task<ActionResult> GetLogins(string username, DataTablesModel model)
		{
			return Json(await AdminUserReader.GetUserLogins(username, model));
		}
	}
}