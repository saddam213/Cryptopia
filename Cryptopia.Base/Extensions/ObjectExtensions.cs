using System;
namespace Cryptopia.Base
{
	public static class ObjectExtensions
	{

		public static DateTime RoundToNearestInterval(this DateTime dt, TimeSpan d)
		{
			int f = 0;
			double m = (double)(dt.Ticks % d.Ticks) / d.Ticks;
			if (m >= 0.5)
				f = 1;
			return new DateTime(((dt.Ticks / d.Ticks) + f) * d.Ticks);
		}

		public static string ElapsedTime(this DateTime dtEvent)
		{
			TimeSpan TS = DateTime.UtcNow - dtEvent;
			int intYears = DateTime.UtcNow.Year - dtEvent.Year;
			int intMonths = DateTime.UtcNow.Month - dtEvent.Month;
			int intDays = DateTime.UtcNow.Day - dtEvent.Day;
			int intHours = DateTime.UtcNow.Hour - dtEvent.Hour;
			int intMinutes = DateTime.UtcNow.Minute - dtEvent.Minute;
			int intSeconds = DateTime.UtcNow.Second - dtEvent.Second;
			if (intYears > 0) return String.Format("{0} {1} ago", intYears, (intYears == 1) ? "year" : "years");
			else if (intMonths > 0) return String.Format("{0} {1} ago", intMonths, (intMonths == 1) ? "month" : "months");
			else if (intDays > 0) return String.Format("{0} {1} ago", intDays, (intDays == 1) ? "day" : "days");
			else if (intHours > 0) return String.Format("{0} {1} ago", intHours, (intHours == 1) ? "hour" : "hours");
			else if (intMinutes > 0) return String.Format("{0} {1} ago", intMinutes, (intMinutes == 1) ? "minute" : "minutes");
			else if (intSeconds > 0) return String.Format("{0} {1} ago", intSeconds, (intSeconds == 1) ? "second" : "seconds");
			else
			{
				return String.Format("{0} {1} ago", dtEvent.ToShortDateString(), dtEvent.ToShortTimeString());
			}
		}

		public static long ToUnixTime(this DateTime date)
		{
			var timeSpan = (date - new DateTime(1970, 1, 1, 0, 0, 0));
			return (long)timeSpan.TotalSeconds;
		}

		public static long ToJavaTime(this DateTime date)
		{
			var timeSpan = (date - new DateTime(1970, 1, 1, 0, 0, 0));
			return (long)timeSpan.TotalMilliseconds;
		}

		public static DateTime ToDateTime(this uint time)
		{
			return new DateTime(1970, 1, 1).AddSeconds(time);
		}

		public static DateTime ToDateTime(this long time)
		{
			return new DateTime(1970, 1, 1).AddSeconds(time);
		}

		public static string GetBytesReadable(this long value)
		{
			// Get absolute value
			long absolute_i = (value < 0 ? -value : value);
			// Determine the suffix and readable value
			string suffix;
			double readable;
			if (absolute_i >= 0x1000000000000000) // Exabyte
			{
				suffix = "EB";
				readable = (value >> 50);
			}
			else if (absolute_i >= 0x4000000000000) // Petabyte
			{
				suffix = "PB";
				readable = (value >> 40);
			}
			else if (absolute_i >= 0x10000000000) // Terabyte
			{
				suffix = "TB";
				readable = (value >> 30);
			}
			else if (absolute_i >= 0x40000000) // Gigabyte
			{
				suffix = "GB";
				readable = (value >> 20);
			}
			else if (absolute_i >= 0x100000) // Megabyte
			{
				suffix = "MB";
				readable = (value >> 10);
			}
			else if (absolute_i >= 0x400) // Kilobyte
			{
				suffix = "KB";
				readable = value;
			}
			else
			{
				return value.ToString("0 B"); // Byte
			}
			// Divide by 1024 to get fractional value
			readable = (readable / 1024);
			// Return formatted number with suffix
			return readable.ToString("0.### ") + suffix;
		}

		private static Random _rnd = new Random();
		public static int GetRandomNumber()
		{
			return _rnd.Next(10000000, 99999999);
		}
	}
}
