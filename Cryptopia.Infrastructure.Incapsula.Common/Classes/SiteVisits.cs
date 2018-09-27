using System.Collections.Generic;

using Cryptopia.Infrastructure.Incapsula.Common.Classes.Visits;
using Cryptopia.Infrastructure.Incapsula.Common.Responses;

namespace Cryptopia.Infrastructure.Incapsula.Common.Classes
{
  public class SiteVisits : ResponseData
  {
    public SiteVisits(SiteVisitsResponse response) : base(response)
    {
      Visits = response.Visits;
    }

    public List<Visit> Visits { get; private set; }
  }
}
