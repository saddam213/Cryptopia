using System.ComponentModel;

namespace Cryptopia.Infrastructure.Incapsula.Common.Enums
{
  public enum StatisticGranularity
  {
    None = 0,
    [Description("259200000 ")]
    UpToThirtyDays,
    [Description("259200000")]
    UpToNinetyDays,
    Custom
  }
}
