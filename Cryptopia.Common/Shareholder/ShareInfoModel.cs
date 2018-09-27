using System;
using System.Collections.Generic;

namespace Cryptopia.Common.Shareholder
{
	public class ShareholderModel
	{
		public ShareholderModel()
		{
			TotalShares = 20000;
			FeeInfo = new List<ShareholderFeeInfo>();
			SiteExpenses = new List<SiteExpenseModel>();
			PaytopiaInfo = new List<ShareholderFeeInfo>();
		}
		public List<ShareholderFeeInfo> FeeInfo { get; set; }
		public List<ShareholderFeeInfo> PaytopiaInfo { get; set; }
		public DateTime LastPayout { get; set; }
		public DateTime NextPayout { get; set; }
		public List<SiteExpenseModel> SiteExpenses { get; set; }
		public int ShareCount { get;  set; }
		public int TotalShares { get; protected set; }
		public decimal BTCDollar { get; set; }
		public decimal TotalExpense { get; set; }
		public decimal TotalBTCExpense { get; set; }
	}
}