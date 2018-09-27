using System.ComponentModel;

namespace Cryptopia.Infrastructure.Incapsula.Common.Enums
{
  public enum TargetSite
  {
    None = 0,
    [Description("Cryptopia Production")]
    Cryptopia_Production = 1,
    [Description("Cryptopia Development")]
    Cryptopia_Development = 2
  }
}
