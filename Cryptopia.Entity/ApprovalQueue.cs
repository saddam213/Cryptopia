using Cryptopia.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptopia.Entity
{
	public class ApprovalQueue
	{
		[Key]
		public int Id { get; set; }
		public string DataUserId { get; set; }
		public string RequestUserId { get; set; }
		public string ApproveUserId { get; set; }
		public ApprovalQueueType Type { get; set; }
		public ApprovalQueueStatus Status { get; set; }
		public string Data { get; set; }
		public DateTime Created { get; set; }
		public DateTime Approved { get; set; }
		public string Message { get; set; }

		[ForeignKey("DataUserId")]
		public virtual ApplicationUser DataUser { get; set; }

		[ForeignKey("RequestUserId")]
		public virtual ApplicationUser RequestUser { get; set; }

		[ForeignKey("ApproveUserId")]
		public virtual ApplicationUser ApproveUser { get; set; }
	}
}
