using System;
using System.ComponentModel;
using System.Reflection;

using Cryptopia.Infrastructure.Incapsula.Common.Constants;
using Cryptopia.Infrastructure.Incapsula.Common.Enums;

namespace Cryptopia.Infrastructure.Incapsula.Common.Extensions
{
  public class EnumerationExtensions
  {
    /// <summary>
    /// Gets the string from the enumerations description attribute if present.
    /// </summary>
    /// <param name="value">The enumeration value to get the description for.</param>
    /// <returns>The description attribute value if present, the value to string otherwise.</returns>
    public static string GetDescription(Enum value)
    {
      FieldInfo info = value.GetType().GetField(value.ToString());

      DescriptionAttribute[] attributes = (DescriptionAttribute[])info.GetCustomAttributes(typeof(DescriptionAttribute), false);

      if (attributes == null || attributes.Length == 0)
        return value.ToString();

      return attributes[0].Description;
    }
  }

  public static class SecurityRuleEnumExtensions
  {
    public static string Name(this SecurityRule rule)
    {
      return EnumerationExtensions.GetDescription(rule);
    }
  }

  public static class AclRuleEnumExtensions
  {
    public static string Name(this AclRule rule)
    {
      return EnumerationExtensions.GetDescription(rule);
    }
  }

  public static class SiteEnumExtensions
  {
    public static int GetSiteId(this TargetSite site)
    {
      switch (site)
      {
        case TargetSite.Cryptopia_Production:
          return EnvironmentConstants.ProductionSiteId;
        case TargetSite.Cryptopia_Development:
          return EnvironmentConstants.DevelopmentSiteId;
        default:
          return -1;
      }
    }
  }
}
