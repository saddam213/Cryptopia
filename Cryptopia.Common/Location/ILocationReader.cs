using System.Collections.Generic;

namespace Cryptopia.Common.Location
{
	public interface ILocationReader
	{
		List<LocationModel> GetLocations();
	}
}
