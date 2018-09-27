using System;
using System.Collections.Generic;
using System.Linq;
using Web.Site.Models;
using System.Web.Mvc;
using Cryptopia.Common.Location;
using Cryptopia.Common.Marketplace;
using Cryptopia.Cache;
using System.Configuration;

namespace Web.Site.Cache
{
	public static class StaticDataCache
	{
		#region Cache

		private static readonly System.Runtime.Caching.MemoryCache _memorycache = new System.Runtime.Caching.MemoryCache("StaticDataCache");

		public static T GetOrSet<T>(string key, Func<T> valueFactory, TimeSpan expiry)
		{
			try
			{
				var newValue = new Lazy<T>(valueFactory);
				var oldValue = _memorycache.AddOrGetExisting(key, newValue, DateTimeOffset.UtcNow.Add(expiry)) as Lazy<T>;
				return (oldValue ?? newValue).Value;
			}
			catch
			{
				_memorycache.Remove(key);
				return default(T);
			}
		}

		#endregion

		#region Locations

		private static ILocationReader _locationReader;

		public static List<LocationModel> Locations
		{
			get { return GetOrSet("Locations", () => GetLocations(), TimeSpan.FromHours(24)); }
		}

		private static List<LocationModel> GetLocations()
		{
			if (_locationReader == null)
				_locationReader = DependencyResolver.Current.GetService<ILocationReader>();

			if (_locationReader != null)
				return _locationReader.GetLocations();

			return new List<LocationModel>();
		}

		#endregion



		public static List<string> Countries
		{
			get
			{
				return GetOrSet("Countries", () =>
					StaticDataCache.Locations
						.Where(x => x.ParentId == 0)
						.Select(x => x.Name)
						.OrderBy(x => x)
						.ToList(), TimeSpan.FromHours(24));
			}
		}

		public static string GetLocation(this MarketplaceItemModel marketItem)
		{
			if (marketItem != null)
			{
				var location = StaticDataCache.Locations.FirstOrDefault(x => x.Id == marketItem.LocationId);
				if (location != null)
				{
					var parent = StaticDataCache.Locations.FirstOrDefault(x => x.Id == location.ParentId && x.Id != location.Id);
					if (parent != null)
					{
						return string.Join(", ", parent.Name, location.Name, marketItem.LocationRegion);
					}
					return location.Name;
				}
			}
			return "Unknown";
		}

		public static string GetCity(this MarketplaceItemModel marketItem)
		{
			if (marketItem != null)
			{
				var location = StaticDataCache.Locations.FirstOrDefault(x => x.Id == marketItem.LocationId);
				if (location != null)
				{
					var parent = StaticDataCache.Locations.FirstOrDefault(x => x.Id == location.ParentId && x.Id != location.Id);
					if (parent != null)
					{
						return location.Name;
					}
				}
			}
			return string.Empty;
		}

		public static string GetCountry(this MarketplaceItemModel marketItem)
		{
			if (marketItem != null)
			{
				var location = StaticDataCache.Locations.FirstOrDefault(x => x.Id == marketItem.LocationId);
				if (location != null)
				{
					var parent = StaticDataCache.Locations.FirstOrDefault(x => x.Id == location.ParentId && x.Id != location.Id);
					if (parent != null)
					{
						return parent.Name;
					}
					return location.Name;
				}
			}
			return "Unknown";
		}

		public static int GetCityId(this MarketplaceItemModel marketItem)
		{
			if (marketItem != null)
			{
				var location = StaticDataCache.Locations.FirstOrDefault(x => x.Id == marketItem.LocationId);
				if (location != null)
				{
					var parent = StaticDataCache.Locations.FirstOrDefault(x => x.Id == location.ParentId && x.Id != location.Id);
					if (parent != null)
					{
						return location.Id;
					}
				}
			}
			return -2;
		}

		public static int GetCountryId(this MarketplaceItemModel marketItem)
		{
			if (marketItem != null)
			{
				var location = StaticDataCache.Locations.FirstOrDefault(x => x.Id == marketItem.LocationId);
				if (location != null)
				{
					var parent = StaticDataCache.Locations.FirstOrDefault(x => x.Id == location.ParentId && x.Id != location.Id);
					if (parent != null)
					{
						return parent.Id;
					}
					return location.Id;
				}
			}
			return -2;
		}


		public static void SetCategories(this EditMarketItemModel model, MarketplaceItemModel marketItem)
		{
			if (marketItem != null)
			{
				var cat1 = model.Categories.FirstOrDefault(x => x.Id == marketItem.CategoryId);
				var cat2 = model.Categories.FirstOrDefault(x => x.Id == (cat1 == null ? -1 : cat1.ParentId) && x.Id > 0);
				var cat3 = model.Categories.FirstOrDefault(x => x.Id == (cat2 == null ? -1 : cat2.ParentId) && x.Id > 0);
				if (cat1 == null)
				{
					model.MainCategoryId = -2;
				}
				else if (cat2 == null)
				{
					model.MainCategoryId = cat1.Id;
				}
				else if (cat3 == null)
				{
					model.SubCategoryId = cat1.Id;
					model.MainCategoryId = cat2.Id;
				}
				else
				{
					model.SubCategoryId = cat1.Id;
					model.CategoryId = cat2.Id;
					model.MainCategoryId = cat3.Id;
				}
			}
		}

	}
}