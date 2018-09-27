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
	public class PoolListingModel
	{
		public decimal Balance { get; set; }
		public string Currency { get; set; }
		public string Description { get; set; }
		public string Name { get; set; }
		public decimal Price { get; set; }

		[Display(Name = nameof(Resources.Paytopia.poolListingIsListedLabel), ResourceType = typeof(Resources.Paytopia))]
		public bool IsListed { get; set; }

		public List<PoolListingItemModel> Items { get; set; }
		public List<AlgoType> SupportedAlgos { get; set; } = new List<AlgoType>(Constant.SupportedPoolAlgos);

		[Display(Name = nameof(Resources.Paytopia.poolListingPoolLabel), ResourceType = typeof(Resources.Paytopia))]
		public int? ItemId { get; set; }

		[RequiredBase]
		[Display(Name = nameof(Resources.Paytopia.poolListingNameLabel), ResourceType = typeof(Resources.Paytopia))]
		public string ListingName { get; set; }

		[Display(Name = nameof(Resources.Paytopia.poolListingSymbolLabel), ResourceType = typeof(Resources.Paytopia))]
		public string ListingSymbol { get; set; }

		[Display(Name = nameof(Resources.Paytopia.poolListingAlgorithmLabel), ResourceType = typeof(Resources.Paytopia))]
		public AlgoType ListingAlgoType { get; set; }

		[Display(Name = nameof(Resources.Paytopia.poolListingWebsiteLabel), ResourceType = typeof(Resources.Paytopia))]
		public string ListingWebsite { get; set; }
		
		[Display(Name = nameof(Resources.Paytopia.poolListingSourceLabel), ResourceType = typeof(Resources.Paytopia))]
		public string ListingSource { get; set; }
		
		[Display(Name = nameof(Resources.Paytopia.poolListingExplorerLabel), ResourceType = typeof(Resources.Paytopia))]
		public string ListingBlockExplorer { get; set; }

		[Display(Name = nameof(Resources.Paytopia.poolListingForumLabel), ResourceType = typeof(Resources.Paytopia))] 
		public string ListingLaunchForum { get; set; }

		[Display(Name = nameof(Resources.Paytopia.poolListingExtraLabel), ResourceType = typeof(Resources.Paytopia))]		
		public string ListingExtraInfo { get; set; }

		[Display(Name = nameof(Resources.Paytopia.poolListingNetworkTypeLabel), ResourceType = typeof(Resources.Paytopia))]
		public NetworkType ListingNetworkType { get; set; }
	}

	public class PoolListingItemModel
	{
		public int Id { get; set; }
		public string Name { get; set; }
	}
}
