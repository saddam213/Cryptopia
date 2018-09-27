using System;

namespace Cryptopia.Common.Marketplace
{
	public class CreateMarketAnswerModel
	{
		public string Answer { get; set; }
		public int QuestionId { get; set; }
		public Guid QuestionUserId { get; set; }
	}
}
