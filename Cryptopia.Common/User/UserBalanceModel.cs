using Cryptopia.Enums;
using System;
using System.Collections.Generic;

namespace Cryptopia.Common.User
{
	public class UserBalanceModel
	{
		public UserBalanceModel()
		{
			Balances = new List<UserBalanceItemModel>();
		}
		public decimal BTCEstimate { get; set; }
		public decimal BTCEstimateAlt { get; set; }
		public List<UserBalanceItemModel> Balances { get; set; }

		public decimal WithdrawLimit { get; set; }
		public decimal WithdrawTotal { get; set; }
		public bool HasWithdrawLimit { get; set; }

		public int WithdrawPercent
		{
			get
			{
				if (WithdrawLimit > 0 && WithdrawTotal > 0)
				{
					return (int)Math.Min(100, Math.Abs((WithdrawTotal / WithdrawLimit) * 100));
				}
				return 0;
			}
		}
	}
}
