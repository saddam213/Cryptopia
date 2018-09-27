using System;
using System.ComponentModel.DataAnnotations;

namespace Cryptopia.Entity
{
    public class ActivityLog
    {
        public ActivityLog()
        { }

        public long Id { get; set; }
        public DateTime DateCreated { get; set; }
        public Guid TxId { get; set; }
        [MaxLength(20)]
        public string Entity { get; set; }
        [MaxLength(32)]
        public string Property{ get; set; }
        [MaxLength(256)]
        public string OldValue { get; set; }
        [MaxLength(256)]
        public string NewValue { get; set; }
        public Guid? UserId { get; set; }
        public Guid? AdminUserId { get; set; }
    }
}
