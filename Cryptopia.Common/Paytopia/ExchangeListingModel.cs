using Cryptopia.Common.Validation;
using Cryptopia.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptopia.Common.Paytopia
{
	public class ExchangeListingModel
	{
		public decimal Balance { get; set; }
		public string Currency { get; set; }
		public string Description { get; set; }
		public string Name { get; set; }
		public decimal Price { get; set; }


		[RequiredBase]
		[StringLengthBase(20, MinimumLength = 1)]
		[Display(Name = nameof(Resources.Paytopia.exchangeNameLabel), ResourceType = typeof(Resources.Paytopia))]
		public string ListingName { get; set; }

		[RequiredBase]
		[StringLengthBase(5, MinimumLength = 1)]
		[Display(Name = nameof(Resources.Paytopia.exchangeTickerLabel), ResourceType = typeof(Resources.Paytopia))]
		public string ListingSymbol { get; set; }

		[RequiredBase]
		[Display(Name = nameof(Resources.Paytopia.exchangeAlgorithmLabel), ResourceType = typeof(Resources.Paytopia))]
		public AlgoType? ListingAlgoType { get; set; }

		[RequiredBase]
		[Display(Name = nameof(Resources.Paytopia.exchangeNetworkTypeLabel), ResourceType = typeof(Resources.Paytopia))]
		public NetworkType? ListingNetworkType { get; set; }

		[Url]
		[StringLengthBase(1000)]
		[Display(Name = nameof(Resources.Paytopia.exchangeCurrencyWebsiteLabel), ResourceType = typeof(Resources.Paytopia))]
		public string ListingWebsite { get; set; }

		[Url]
		[RequiredBase]
		[StringLengthBase(1000)]
		[Display(Name = nameof(Resources.Paytopia.exchangeSourceCodeLinkLabel), ResourceType = typeof(Resources.Paytopia))]
		public string ListingSource { get; set; }

		[Url]
		[StringLengthBase(1000)]
		[Display(Name = nameof(Resources.Paytopia.exchangeBlockExplorerLinkLabel), ResourceType = typeof(Resources.Paytopia))]
		public string ListingBlockExplorer { get; set; }

		[Url]
		[RequiredBase]
		[StringLengthBase(1000)]
		[Display(Name = nameof(Resources.Paytopia.exchangeLaunchForumThreadLabel), ResourceType = typeof(Resources.Paytopia))]
		public string ListingLaunchForum { get; set; }

		[StringLengthBase(1000)]
		[Display(Name = nameof(Resources.Paytopia.exchangeExtraInformationLabel), ResourceType = typeof(Resources.Paytopia))]
		public string ListingExtraInfo { get; set; }
	}
}
