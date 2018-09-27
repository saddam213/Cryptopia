using System.ComponentModel;

namespace Cryptopia.Infrastructure.Incapsula.Common.Enums
{
  public enum TimeRange
  {
    None = 0,
    [Description("today")]
    Today = 1,
    [Description("last_7_days")]
    LastSevenDays = 2,
    [Description("last_30_days")]
    LastThirtyDays = 3,
    [Description("last_90_days")]
    LastNinetyDays = 4,
    [Description("month_to_date")]
    MonthToDate = 5,
    [Description("custom")]
    Custom = 6
  }
}
