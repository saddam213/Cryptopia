
namespace Cryptopia.Infrastructure.Incapsula.Common.Enums
{
  public enum ResponseCode
  {
    Success = 0,
    UnexpectedError = 1,
    InvalidInput = 2,
	  OperationTimeOutOrServerUnavailable = 4,
    EmailInvalid = 1001,
    PlanIdInvalid = 1003,
    AccountExists = 1010,
    DomainInvalid = 3001,
    SiteIsOnTheCloudFlareNetwork = 3002,
    SiteRequiresSSL = 3003,
    DomainBelongsToAKnownService = 3004,
    SiteIsOnAServiceThatIsNotSupportedByAccountPlan = 3005,
    SiteRequiresMultipleIPSupport = 3006,
    SiteUnresolvable = 3011,
    SiteUnreachable = 3012,
    SiteAlreadyProtectedByService = 3013,
    NumberOfAllowedSitesExceeded = 3014,
    InternalError = 3015,
    ReportInvalid = 5001,
    FormatInvalid = 5002,
    PatternInvalid = 5010,
    InvalidConfigParameterName= 6001,
    InvalidConfigParameterValue = 6002,
    ActionRequired = 6003,
    UnknownOrUnauthorizedAccountId = 9403,
    AuthenticationMissingOrInvalid = 9411,
	  Unknown_UnauthorizedSiteId = 9413,
    FeatureNotPermitted = 9414,
    OperationNotAllowed = 9415,
    TimeRangeInvalid = 13001,
    GranularityInvalid = 13002,
    InvalidPublicKey = 13007,
    InvalidConfigurationId = 13008,
    InsufficientKeyLength = 13009,
    UnknownResponse = 19000
  }
}
