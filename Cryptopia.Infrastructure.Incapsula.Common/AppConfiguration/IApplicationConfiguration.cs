using Cryptopia.Infrastructure.Incapsula.Common.Enums;

namespace Cryptopia.Infrastructure.Incapsula.Common.AppConfiguration
{
    public interface IApplicationConfiguration
    {
        TargetSite TargetWebsite { get; }
        string IncapsulaApiId { get; }
        string IncapsulaApiKey { get; }
        string IncapsulaAccountId { get; }
        string IncapsulaEndpoint { get; }
        int IncapsulaApiPollInterval { get; }
    }
}
