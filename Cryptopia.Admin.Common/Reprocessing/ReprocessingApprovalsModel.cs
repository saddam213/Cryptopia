using System;
using Cryptopia.Enums;
using System.ComponentModel;

namespace Cryptopia.Admin.Common.Reprocessing
{
    public class ReprocessingApprovalsModel
    {
        public int Id { get; set; }
        public ApprovalQueueType Type{ get; set; }
        public ApprovalQueueStatus Status { get; set; }
        public string RequestedBy { get; set; }
        public DateTime Requested { get; set; }
        [DisplayName("Approved By:")]
        public string ApprovedBy { get; set; }
        public DateTime Approved { get; set; }
    }
}
