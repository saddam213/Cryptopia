using System.Collections.Generic;

namespace Cryptopia.Infrastructure.Incapsula.Common.Classes.Visits
{
  public class Visit
  {
    public string id { get; set; }
    public int siteId { get; set; }
    public long startTime { get; set; }
    public long endTime { get; set; }
    public List<string> clientIPs { get; set; }
    public List<string> country { get; set; }
    public List<string> countryCode { get; set; }
    public string clientType { get; set; }
    public string clientApplication { get; set; }
    public int clientApplicationId { get; set; }
    public string httpVersion { get; set; }
    public string clientApplicationVersion { get; set; }
    public string userAgent { get; set; }
    public string os { get; set; }
    public string osVersion { get; set; }
    public bool supportsCookies { get; set; }
    public bool supportsJavaScript { get; set; }
    public int hits { get; set; }
    public int pageViews { get; set; }
    public string entryReferer { get; set; }
    public string entryPage { get; set; }
    public List<string> servedVia { get; set; }
    public SecuritySummary securitySummary { get; set; }
    public List<Action> actions { get; set; }
  }
}
