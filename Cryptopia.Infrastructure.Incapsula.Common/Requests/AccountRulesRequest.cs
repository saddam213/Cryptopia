using System.Collections.Generic;

using Cryptopia.Infrastructure.Incapsula.Common.AppConfiguration;

namespace Cryptopia.Infrastructure.Incapsula.Common.Requests
{
  public class AccountRulesRequest : RequestBase
  {
    public AccountRulesRequest(IApplicationConfiguration config) : base(config)
    { }

    public override string APITarget => $"/sites/incapRules/account/list";

    public override Dictionary<string, string> RequestValues
    {
      get
      {
        var values = base.RequestValues;
        values.Add("account_id", AccountId);
        values.Add("include_ad_rules", $"{IncludeAdRules}");
        values.Add("include_incap_rules", $"{IncludeIncapRules}");
        return values;
      }
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
  }
}
