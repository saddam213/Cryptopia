using Cryptopia.Infrastructure.Incapsula.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptopia.Admin.Common.ActivityLogging
{
    public class ActivityTrendData
    {
        public ActivityTrendData()
        {
            Initialize();
        }

        #region Verifications

        public ICollection<ActivityDataPoint> ApprovedVerifications { get; set; }
        public ICollection<ActivityDataPoint> RejectedVerifications { get; set; }
        public ICollection<ActivityDataPoint> OverallVerifications { get; set; }

        #endregion

        #region Support Tickets

        public ICollection<ActivityDataPoint> ClosedSupportTickets { get; set; }
        public ICollection<ActivityDataPoint> UpdatedSupportTickets { get; set; }
        public ICollection<ActivityDataPoint> OverallOpenTickets { get; set; }

        #endregion

        private void Initialize()
        {
            if (ApprovedVerifications == null)
                ApprovedVerifications = new List<ActivityDataPoint>();

            if (RejectedVerifications == null)
                RejectedVerifications = new List<ActivityDataPoint>();

            if (OverallVerifications == null)
                OverallVerifications = new List<ActivityDataPoint>();

            if (ClosedSupportTickets == null)
                ClosedSupportTickets = new List<ActivityDataPoint>();

            if (UpdatedSupportTickets == null)
                UpdatedSupportTickets = new List<ActivityDataPoint>();

            if (OverallOpenTickets == null)
                OverallOpenTickets = new List<ActivityDataPoint>();
        }
    }
}
