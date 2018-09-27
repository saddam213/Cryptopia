using System;
using System.Globalization;

namespace Cryptopia.Base.Extensions
{
	public static class DateExtensions
	{
		public static string GetTimeToGo(this DateTime end)
		{
			if (end > DateTime.UtcNow)
			{
				var timespan = end - DateTime.UtcNow;
				if (timespan.TotalDays > 0)
				{
					return new TimeSpan(timespan.Days, timespan.Hours, 0, 0).ToReadableString();
				}
				else if (timespan.TotalHours > 0)
				{
					return new TimeSpan(timespan.Days, timespan.Hours, timespan.Minutes, 0).ToReadableString();
				}
				return timespan.ToReadableString();
			}
			return string.Empty;
		}

		public static string ToReadableString(this TimeSpan span)
		{
			var formatted = string.Format("{0}{1}{2}{3}",
					span.Duration().Days > 0 ? string.Format("{0:0} day{1}, ", span.Days, span.Days == 1 ? string.Empty : "s") : string.Empty,
					span.Duration().Hours > 0 ? string.Format("{0:0} hour{1}, ", span.Hours, span.Hours == 1 ? string.Empty : "s") : string.Empty,
					span.Duration().Minutes > 0 ? string.Format("{0:0} minute{1}, ", span.Minutes, span.Minutes == 1 ? string.Empty : "s") : string.Empty,
					span.Duration().Seconds > 0 ? string.Format("{0:0} second{1}", span.Seconds, span.Seconds == 1 ? string.Empty : "s") : string.Empty);

			if (formatted.EndsWith(", ")) formatted = formatted.Substring(0, formatted.Length - 2);

			if (string.IsNullOrEmpty(formatted)) formatted = "0 seconds";

			return formatted;
		}

		public static string ToReadableStringShort(this TimeSpan span)
		{
			var formatted = string.Format("{0}{1}{2}{3}",
					span.Duration().Days > 0 ? string.Format("{0:0}d,", span.Days) : string.Empty,
					span.Duration().Hours > 0 ? string.Format("{0:0}h,", span.Hours) : string.Empty,
					span.Duration().Minutes > 0 ? string.Format("{0:0}m,", span.Minutes) : string.Empty,
					span.Duration().Seconds > 0 ? string.Format("{0:0}s", span.Seconds) : string.Empty);

			return !string.IsNullOrEmpty(formatted)
					? formatted.TrimEnd(',') : "0s";
		}

		public static int WeekOfYear(this DateTime dateTime)
		{
			return CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(dateTime, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
		}

		public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
		{
			var diff = dt.DayOfWeek - startOfWeek;
			if (diff < 0)
			{
				diff += 7;
			}
			return dt.AddDays(-1 * diff).Date;
		}

		public static DateTime RoundUp(this DateTime dt, TimeSpan d)
		{
			var modTicks = dt.Ticks % d.Ticks;
			var delta = modTicks != 0 ? d.Ticks - modTicks : 0;
			return new DateTime(dt.Ticks + delta, dt.Kind);
		}

		public static DateTime RoundDown(this DateTime dt, TimeSpan d)
		{
			var delta = dt.Ticks % d.Ticks;
			return new DateTime(dt.Ticks - delta, dt.Kind);
		}

		public static DateTime RoundToNearest(this DateTime dt, TimeSpan d)
		{
			var delta = dt.Ticks % d.Ticks;
			bool roundUp = delta > d.Ticks / 2;
			var offset = roundUp ? d.Ticks : 0;

			return new DateTime(dt.Ticks + offset - delta, dt.Kind);
		}

	}


}
