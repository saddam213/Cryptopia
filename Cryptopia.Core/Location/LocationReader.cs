using Cryptopia.Infrastructure.Common.DataContext;
using Cryptopia.Common.Location;
using System.Collections.Generic;
using System.Linq;
using Cryptopia.Common.DataContext;

namespace Cryptopia.Core.Location
{
	public class LocationReader : ILocationReader
	{
		public IExchangeDataContextFactory DataContextFactory { get; set; }

		public List<LocationModel> GetLocations()
		{
			using (var context = DataContextFactory.CreateReadOnlyContext())
			{
				return context.Location
				.AsNoTracking()
				.Select(l => new LocationModel
				{
					Id = l.Id,
					Name = l.Name,
					ParentId = l.ParentId,
					CountryCode = l.CountryCode
				})
				.OrderBy(l => l.Name)
				.ToListNoLock();
			}
		}

	}
}
