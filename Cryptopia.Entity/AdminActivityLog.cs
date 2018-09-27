using System;
using System.ComponentModel.DataAnnotations;

namespace Cryptopia.Entity
{
    public class AdminActivityLog
    {
        public AdminActivityLog()
        { }

        [Key]
        public long Id { get; set; }
        public Guid AdminUserId { get; set; }
        public DateTime DateCreated { get; set; }
        public string ActivityDescription { get; set; }
    }
}
