using Cryptopia.Enums;
using System;
using System.Collections.Generic;

namespace Cryptopia.Common.Lotto
{
	public class LottoItemModel
	{
		public int LottoItemId { get; set; }
		public string Symbol { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public LottoType LottoType { get; set; }
		public decimal Fee { get; set; }
		public decimal Rate { get; set; }
		public int Hours { get; set; }
		public int Prizes { get; set; }
		public DateTime NextDraw { get; set; }
		public int TicketsInDraw { get; set; }
		public int UserTicketsInDraw { get; set; }
		public int CurrentDrawId { get; set; }
		public List<LottoPrizeInfoModel> PrizeInfo { get; set; }
		public int CurrencyId { get; set; }
		public LottoItemStatus Status { get; set; }
		public decimal PrizePool { get; set; }
	}
}
