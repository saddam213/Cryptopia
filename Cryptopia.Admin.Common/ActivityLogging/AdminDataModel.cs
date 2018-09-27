using System;
using System.Collections.Generic;
using System.Linq;

namespace Cryptopia.Admin.Common.ActivityLogging
{
    public class AdminDataModel
    {
        List<AdminAction> _actions = new List<AdminAction>();
        List<AdminAction> _approvedVerifications;
        List<AdminAction> _rejectedVerifications;
        List<AdminAction> _closedSupportTickets;
        List<AdminAction> _updatedSupportTickets;
        List<AdminAction> _createdSupportQueues;
        List<AdminAction> _createdSupportTags;
        List<AdminAction> _createdTradePairs;
        List<AdminAction> _updatedTradePairs;
        List<AdminAction> _closedTradePairs;
        List<AdminAction> _activatedUsers;
        List<AdminAction> _lockedOutUsers;
        List<AdminAction> _disabledUsers;

        public DateTime DateOccurred { get; set; }

        public List<AdminAction> Actions
        {
            get { return _actions; }
            set
            {
                _actions = value;

                if (_actions != null)
                {
                    _approvedVerifications = _actions.Where(x => x.ActionDescription.Contains("Accepted User Verification")).ToList();
                    _rejectedVerifications = _actions.Where(x => x.ActionDescription.Contains("Rejected User Verification")).ToList();
                    _closedSupportTickets = _actions.Where(x => x.ActionDescription.Contains("Updated ticket status") && x.ActionDescription.Contains("to Closed")).ToList();
                    _updatedSupportTickets = _actions.Where(x => x.ActionDescription.Contains("Updated ticket status") && !x.ActionDescription.Contains("to Closed")).ToList();
                    _createdSupportQueues = Actions.Where(x => x.ActionDescription.Contains("New Queue created")).ToList();
                    _createdSupportTags = Actions.Where(x => x.ActionDescription.Contains("Created new tag:")).ToList();
                    _createdTradePairs = Actions.Where(x => x.ActionDescription.Contains("Creating new Trade Pair:")).ToList();
                    _updatedTradePairs = Actions.Where(x => x.ActionDescription.Contains("Updating Trade Pair:") && !x.ActionDescription.Contains("New status Closed.")).ToList();
                    _closedTradePairs = Actions.Where(x => x.ActionDescription.Contains("Updating Trade Pair:") && x.ActionDescription.Contains("New status Closed.")).ToList();
                    _activatedUsers = Actions.Where(x => x.ActionDescription.Contains("Activating user")).ToList();
                    _lockedOutUsers = Actions.Where(x => x.ActionDescription.Contains("Locked User")).ToList();
                    _disabledUsers = Actions.Where(x => x.ActionDescription.Contains("Disabling User")).ToList();

                }
            }
        }

        public int VerificationsApproved { get { return ApprovedVerifications.Count(); } }

        public List<AdminAction> ApprovedVerifications
        {
            get
            {
                return _approvedVerifications;
            }
        }

        public int VerificationsRejected { get { return RejectedVerifications.Count(); } }

        public List<AdminAction> RejectedVerifications
        {
            get
            {
                return _rejectedVerifications;
            }
        }

        public int SupportTicketsClosed { get { return ClosedSupportTickets.Count(); } }

        public List<AdminAction> ClosedSupportTickets
        {
            get
            {
                return _closedSupportTickets;
            }
        }

        public int SupportTicketsUpdated { get { return UpdatedSupportTickets.Count(); } }

        public List<AdminAction> UpdatedSupportTickets
        {
            get
            {
                return _updatedSupportTickets;
            }
        }

        public int SupportQueuesCreated { get { return CreatedSupportQueues.Count(); } }

        public List<AdminAction> CreatedSupportQueues
        {
            get
            {
                return _createdSupportQueues;
            }
        }

        public int SupportTagsCreated { get { return CreatedSupportTags.Count(); } }

        public List<AdminAction> CreatedSupportTags
        {
            get
            {
                return _createdSupportTags;
            }
        }

        public List<string> TagsCreated { get { return Actions.Where(x => x.ActionDescription.Contains("Created new tag:")).Select(x => x.ActionDescription.Substring(x.ActionDescription.IndexOf(':') + 1)).ToList(); } }

        public int TradePairsCreated { get { return CreatedTradePairs.Count(); } }

        public List<AdminAction> CreatedTradePairs
        {
            get
            {
                return _createdTradePairs;
            }
        }

        public int TradePairsUpdated { get { return UpdatedTradePairs.Count(); } }

        public List<AdminAction> UpdatedTradePairs
        {
            get
            {
                return _updatedTradePairs;
            }
        }

        public int TradePairsClosed { get { return ClosedTradePairs.Count(); } }

        public List<AdminAction> ClosedTradePairs
        {
            get
            {
                return _closedTradePairs;
            }
        }

        public int UsersActivated { get { return ActivatedUsers.Count(); } }

        public List<AdminAction> ActivatedUsers
        {
            get
            {
                return _activatedUsers;
            }
        }

        public int UsersLockedOut { get { return LockedOutUsers.Count(); } }

        public List<AdminAction> LockedOutUsers
        {
            get
            {
                return _lockedOutUsers;
            }
        }

        public int UsersDisabled { get { return DisabledUsers.Count(); } }

        public List<AdminAction> DisabledUsers
        {
            get
            {
                return _disabledUsers;
            }
        }

    }
}
