using Cryptopia.Common.Validation;
using Cryptopia.Enums;
using System.ComponentModel.DataAnnotations;

namespace Cryptopia.Common.Support
{
	public class CreateSupportTicketModel
	{
		[RequiredBase]
		[Display(Name = nameof(Resources.Support.SubjectLabel), ResourceType = typeof(Resources.Support))]
		public string Subject { get; set; }

		[RequiredBase]
		[Display(Name = nameof(Resources.Support.CategoryLabel), ResourceType = typeof(Resources.Support))]
		public SupportTicketCategory Category { get; set; }

		[RequiredBase]
		[Display(Name = nameof(Resources.Support.DescriptionLabel), ResourceType = typeof(Resources.Support))]
		public string Description { get; set; }

		public int TicketId { get; set; }
		public string Created { get; set; }
		public string CategoryName { get; set; }
	}
}
