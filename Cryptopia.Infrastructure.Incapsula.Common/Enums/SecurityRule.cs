using System.ComponentModel;

namespace Cryptopia.Infrastructure.Incapsula.Common.Enums
{
  public enum SecurityRule
  {
    [Description("api.threats.action.disabled")]
    Disabled = 0,
    [Description("api.threats.action.alert")]
    Alert,
    [Description("api.threats.action.block_request")]
    BlockRequest,
    [Description("api.threats.action.block_user")]
    BlockUser,
    [Description("api.threats.action.block_ip")]
    BlockIp,
    [Description("api.threats.action.quarantine_url")]
    QuarantineUrl
  }
}
