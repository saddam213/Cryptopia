using System.ComponentModel.DataAnnotations;
using Cryptopia.Enums;

namespace Cryptopia.Common.Lotto
{
	public class UpdateLottoItemModel
	{
		public int LottoItemId { get; set; }

		[Required]
		[StringLength(128)]
		public string Name { get; set; }

		[Required]
		[StringLength(4000)]
		public string Description { get; set; }

		public LottoType LottoType { get; set; }

		public LottoItemStatus Status { get; set; }
	}
}