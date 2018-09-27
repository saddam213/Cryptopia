using System.Collections.Generic;

using Cryptopia.Infrastructure.Incapsula.Common.Responses;

namespace Cryptopia.Infrastructure.Incapsula.Common.Classes
{
  public class DomainApproverInformation
  {
    public DomainApproverInformation(DomainApproverInformationResponse response)
    {
      DomainEmails = response.DomainEmails;
    }

    public List<string> DomainEmails { get; set; }
  }
}
