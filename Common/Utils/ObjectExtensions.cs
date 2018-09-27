using System;
namespace Cryptopia.API.Utils
{
	public static class ObjectExtensions
    {
        public static T CopyPropertiesFrom<T>(this T obj1, object obj2)
        {
            foreach (var prop1 in obj1.GetType().GetProperties())
            {
                var partialInfo = obj2.GetType().GetProperty(prop1.Name);
                if (partialInfo != null)
                {
                    prop1.SetValue(obj1,partialInfo.GetValue(obj2, null));
                }
            }
            return obj1;
        }

        //public static T CopyPropertiesTo<T>(this T obj1, U obj2)
        //{
        //    foreach (var prop1 in obj1.GetType().GetProperties())
        //    {
        //        var partialInfo = obj2.GetType().GetProperty(prop1.Name);
        //        if (partialInfo != null)
        //        {
        //            prop1.SetValue(obj1, partialInfo.GetValue(obj2, null));
        //        }
        //    }
        //    return obj1;
        //}

        /// <summary>
        /// Converts UNIX timestamp to DateTime object.
        /// </summary>
        /// <param name="time">The time.</param>
        /// <returns>the time, LOL</returns>
        public static DateTime ToDateTime(this uint time)
        {
            return new DateTime(1970, 1, 1).AddSeconds(time);
        }

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

		public static DateTime ToDateTime(this long time)
		{
			return new DateTime(1970, 1, 1).AddSeconds(time);
		}
    }
}
