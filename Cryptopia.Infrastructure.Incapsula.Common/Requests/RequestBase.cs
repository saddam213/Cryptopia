using System.Collections.Generic;

using Newtonsoft.Json;

using Cryptopia.Infrastructure.Incapsula.Common.AppConfiguration;
using Cryptopia.Infrastructure.Incapsula.Common.Enums;
using Cryptopia.Infrastructure.Incapsula.Common.Extensions;

namespace Cryptopia.Infrastructure.Incapsula.Common.Requests
{
  public class RequestBase
  {
    string _baseEndpoint = string.Empty;
    public IApplicationConfiguration _config = null;

    public RequestBase(IApplicationConfiguration config)
    {
      if (config != null)
      {
        _config = config;
        _baseEndpoint = config.IncapsulaEndpoint;
        APIId = config.IncapsulaApiId;
        APIKey = config.IncapsulaApiKey;
        AccountId = config.IncapsulaAccountId;
      }
    }

    [JsonIgnore]
    public string RequestPath { get { return $"{_baseEndpoint}{Endpoint}{APITarget}"; } }

    [JsonIgnore]
    public virtual Dictionary<string, string> RequestValues
    {
      get
      {
        return new Dictionary<string, string>
        {
          { "api_id", APIId },
          { "api_key", APIKey }
        };
      }
    }

    /// <summary>
    /// The specific Incapsula api category to target for the api call.
    /// IMPORTANT: Must always start with a /
    /// </summary>
    public virtual string Endpoint { get { return "/prov/v1"; } }
    public virtual string APITarget { get {return string.Empty; } }

    public string APIId { get; private set; }
    public string APIKey { get; private set; }
    public string AccountId { get; private set; }

    public string GetSiteId()
    {
      if (_config != null)
      {
        switch (_config.TargetWebsite)
        {
          case TargetSite.Cryptopia_Development:
            return $"{TargetSite.Cryptopia_Development.GetSiteId()}";
          case TargetSite.Cryptopia_Production:
            return $"{TargetSite.Cryptopia_Production.GetSiteId()}";
          case TargetSite.None:
          default:
            break;
        }
      }

      return string.Empty;
    }
  }
}
