using System;

namespace Cryptopia.Infrastructure.Incapsula.Common.Extensions
{
  public static class DateTimeExtensions
  {
    public static long ToTotalMilliseconds(this DateTime dateTime)
    {
      DateTime dt1970 = new DateTime(1970, 1, 1);
      TimeSpan span = dateTime - dt1970;
      return (long)Math.Floor(span.TotalMilliseconds);
    }

    public static DateTime FromTotalMilliseconds(this long totalMilliseconds)
    {
      DateTime dt1970 = new DateTime(1970, 1, 1).Add(TimeSpan.FromMilliseconds(totalMilliseconds));
      return dt1970;
    }
  }
}
