namespace Cryptopia.Common.TermDeposits
{
	public class TermDepositItemModel
	{
		public string Description { get; set; }
		public int Id { get; set; }
		public decimal InterestRate { get; set; }
		public decimal Minimum { get; set; }
		public string Name { get; set; }
		public int TermLength { get; set; }
	}
}
