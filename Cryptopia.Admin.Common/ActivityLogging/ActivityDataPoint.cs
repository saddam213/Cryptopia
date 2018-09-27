
namespace Cryptopia.Admin.Common.ActivityLogging
{
    public class ActivityDataPoint
    {
        public ActivityDataPoint(long timeInMilliseconds, int numberOfOccurances)
        {
            Milliseconds = timeInMilliseconds;
            Occurrances = numberOfOccurances;
        }

        public long Milliseconds { get; set; }
        public int Occurrances { get; set; }
    }
}
