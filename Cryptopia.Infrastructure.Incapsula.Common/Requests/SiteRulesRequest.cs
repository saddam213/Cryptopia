using System.Collections.Generic;

using Cryptopia.Infrastructure.Incapsula.Common.AppConfiguration;

namespace Cryptopia.Infrastructure.Incapsula.Common.Requests
{
  public class SiteRulesRequest : RequestBase
  {
    public override string APITarget => "/sites/incapRules/list";

    public override Dictionary<string, string> RequestValues
    {
      get
      {
        var values = base.RequestValues;
        values.Add("site_id", $"{GetSiteId()}");
        values.Add("include_ad_rules", $"{IncludeAdRules}");
        values.Add("include_incap_rules", $"{IncludeIncapRules}");
        values.Add("page_size", $"{PageSize}");
        values.Add("page_num", $"{PageNum}");

        return values;
      }
    }

    public SiteRulesRequest(IApplicationConfiguration config) : base(config)
    { }

    public SiteRulesRequest(IApplicationConfiguration config, bool includeAdRules, bool includeIncapRules, int pageSize, int pageNum) : this(config)
    {
      IncludeAdRules = includeAdRules;
      IncludeIncapRules = includeIncapRules;
      PageSize = pageSize;
      PageNum = pageNum;
    }

    /// <summary>
    /// OPTIONAL: Whether or not delivery rules should be included. 
    /// Defaults to "Yes"
    /// </summary>
    private bool _includeAdRules = true;
    public bool IncludeAdRules
    {
      get { return _includeAdRules; }
      private set
      {
        _includeAdRules = value;
      }
    }

    /// <summary>
    /// OPTIONAL: Should security rules be included. Defaults to yes
    /// </summary>
    private bool _includeIncapRules = true;
    public bool IncludeIncapRules
    {
      get { return _includeIncapRules; }
      private set
      {
        _includeIncapRules = value;
      }
    }

    /// <summary>
    /// OPTIONAL: The number of objects to return in the response. Defaults to 50.
    /// </summary>

    private int _pageSize = 50;
    public int PageSize
    {
      get { return _pageSize; }
      private set
      {
        _pageSize = value;
      }
    }

    /// <summary>
    /// OPTIONAL: The page to return starting from 0. Default to 0.
    /// </summary>
    private int _pageNum = 0;
    public int PageNum
    {
      get { return _pageNum; }
      private set
      {
        _pageNum = value;
      }
    }
  }
}
