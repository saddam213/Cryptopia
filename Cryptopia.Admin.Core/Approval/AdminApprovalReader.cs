using System.Linq;
using System.Threading.Tasks;
using Cryptopia.Admin.Common.Approval;
using Cryptopia.Infrastructure.Common.DataContext;
using Cryptopia.Infrastructure.Common.DataTables;
using Cryptopia.Admin.Common.AdminUser;
using Newtonsoft.Json;
using System;

namespace Cryptopia.Admin.Core.Approval
{
	public class AdminApprovalReader : IAdminApprovalReader
	{
		public IDataContextFactory DataContextFactory { get; set; }

		public async Task<AdminApprovalModel> GetApproval(int approvalId)
		{
			using (var context = DataContextFactory.CreateContext())
			{
                var approvalQueue = await context.ApprovalQueue
                    .FirstOrDefaultNoLockAsync(x => x.Id == approvalId);

                var approval = new AdminApprovalModel
				{
					Id = approvalQueue.Id,
					Type = approvalQueue.Type,
					Data = DeserializeNewEmailAddress(approvalQueue.Data),
					Status = approvalQueue.Status,
					Created = approvalQueue.Created,
					Approved = approvalQueue.Approved,
					RequestUser = approvalQueue.RequestUser.UserName,
					DataUser = approvalQueue.DataUser.UserName,
					Message = approvalQueue.Message,
					ApprovalUser = approvalQueue.ApproveUser == null ? string.Empty : approvalQueue.ApproveUser.UserName,
				};

                return approval;
			}
		}

        private string DeserializeNewEmailAddress(string data)
        {
            try
            {
                var model = JsonConvert.DeserializeObject<AdminChangeEmailModel>(data);

                if (model != null)
                    return model.NewEmailAddress;
            }
            catch (Exception)
            { }

            return data;
        }

		public async Task<DataTablesResponse> GetApprovals(DataTablesModel model)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var query = context.ApprovalQueue
					.Select(x => new
					{
						Id = x.Id,
						Type = x.Type,
						User = x.DataUser.UserName,
						RequestBy = x.RequestUser.UserName,
						Requested = x.Created,
						Status = x.Status,
						Approved = x.Approved,
						ApprovedBy = x.ApproveUser == null ? string.Empty : x.ApproveUser.UserName,
					}).OrderByDescending(x => x.Id);

				return await query.GetDataTableResultNoLockAsync(model);
			}
		}
	}
}