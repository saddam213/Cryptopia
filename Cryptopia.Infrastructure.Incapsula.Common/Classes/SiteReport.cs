using System;
using System.Text;

using Cryptopia.Infrastructure.Incapsula.Common.Responses;

namespace Cryptopia.Infrastructure.Incapsula.Common.Classes
{
  public class SiteReport : ResponseData
  {
    public SiteReport(SiteReportResponse response) : base(response)
    {
      Format = response.Format;
      Report = DecodeReport(response.Report);      
    }

    public string Format { get; private set; }

    public string Report { get; private set; }

    private string DecodeReport(string base64String)
    {
      try
      {
        byte[] bytes = Convert.FromBase64String(base64String);
        return Encoding.UTF8.GetString(bytes);
      }
      catch (Exception)
      { }

      return string.Empty;
    }
  }
}
