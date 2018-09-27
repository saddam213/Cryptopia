using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cryptopia.Entity
{
    public class DataPoint
    {
        [Key]
        public int Id { get; set; }
        public DateTime DateOccurred { get; set; }
        public long DateOccurredMilliseconds { get; set; }
        public int NumberOccurred { get; set; }
    }
}
