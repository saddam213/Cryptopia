namespace Cryptopia.Common.TermDeposits
{
	public class TermDepositSummaryModel
	{
		public string Description { get; set; }
		public int Id { get; set; }
		public decimal InterestRate { get; set; }
		public decimal Minimum { get; set; }
		public string Name { get; set; }
		public int TermLength { get; set; }

		public decimal ActiveInvested { get; set; }
		public decimal TotalInvested { get; set; }
		public int ActiveDeposits { get; set; }
		public int ClosedDeposits { get; set; }
	}
}
