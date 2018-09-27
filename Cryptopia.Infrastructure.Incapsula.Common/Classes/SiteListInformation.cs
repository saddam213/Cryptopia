using System.Collections.Generic;

using Cryptopia.Infrastructure.Incapsula.Common.Responses;

namespace Cryptopia.Infrastructure.Incapsula.Common.Classes
{
  public class SiteListInformation
  {
    public SiteListInformation(SiteListInformationResponse response)
    {
      Sites = response.Sites;
    }

    public List<Site> Sites { get; set; }
  }
}
